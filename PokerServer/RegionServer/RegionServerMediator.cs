using System.Threading.Tasks;
using UniCastCommonData.Handlers;
using RegionServer.Client;
using RegionServer.FrontendDistribution;
using RegionServer.Lobby;
using UniCastCommonData.ServerPool;
using System;

namespace RegionServer
{
	public class RegionServerMediator : AbstractMediator<RegionServerMediator>
	{
		private static Region_Lobby_Server _lobbyServer;
		private static Region_Client_Server _clientServer;

		private static Region_FrontendDistribution _headConnection;

		private static ServerPool<Lobby_Server_Process> _lobbies;

		public RegionServerMediator(int ticks = 30) : base(ticks) { }

		public async override Task StartServers<T>(T param)
		{
			if (typeof(T) != typeof(string[]))
			{
				return;
			}

			string[] args = (string[])Convert.ChangeType(param, typeof(string[]));

			await InitConnectionToFrontendDistributionServer(args);

			await StartLobbyServer();


			var factory = new LobbyServerProcessFactory();

			_lobbies = new ServerPool<Lobby_Server_Process>(() => factory.CreateWithParams(args));
			await _lobbies.CreateNew();
			await _lobbies.CreateNew();
			await _lobbies.CreateNew();

			Task<Region_Client_Server> startClientServer = StartClientServer();
			await startClientServer;
		}

		public static async Task<Region_Lobby_Server> StartLobbyServer()
		{
			return await ServerInitialisator<Region_Lobby_Server>.StartServer(6380, 6480);
		}

		public static async Task<Region_Client_Server> StartClientServer()
		{
			return await ServerInitialisator<Region_Client_Server>.StartServer(7000, 7100);
		}

		public static async Task<Region_FrontendDistribution> InitConnectionToFrontendDistributionServer(string[] args)
		{
			// TCP server address
			string address = "127.0.0.1";
			if (args != null && args.Length > 0)
				address = args[0];

			// TCP server port
			int port = 1111;
			if (args != null && args.Length > 1)
				port = int.Parse(args[1]);

			Region_FrontendDistribution headConnection = new Region_FrontendDistribution(address, port);

			bool isConnected = headConnection.ConnectAsync();

			if (isConnected == false)
			{
				while (headConnection.IsConnected == false)
					await Task.Yield();
			}

			return headConnection;
		}
	}
}
