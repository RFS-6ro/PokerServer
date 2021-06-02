using System.Threading.Tasks;
using RegionServer.Client;
using RegionServer.FrontendDistribution;
using RegionServer.Lobby;
using UniCastCommonData.Handlers;
using UniCastCommonData.ServerPool;

namespace RegionServer
{
	class Program
	{
		private static Region_Lobby_Server _lobbyServer;
		private static Region_Client_Server _clientServer;

		private static Region_FrontendDistribution _headConnection;

		private static ServerPool<Lobby_Server_Process> _lobbies;

		static async void Main(string[] args)
		{
			Task<Region_FrontendDistribution> initConnectionToFrontendDistributionServer = InitConnectionToFrontendDistributionServer(args);
			await initConnectionToFrontendDistributionServer;

			Task<Region_Lobby_Server> startLobbyServer = StartLobbyServer();
			await startLobbyServer;

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
