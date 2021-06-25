using System.Threading.Tasks;
using UniCastCommonData.Handlers;
using LobbyServer.Client;
using LobbyServer.Region;
using System;
using FrontendDistributionServer.Database;

namespace LobbyServer
{
	public class LobbyServerMediator : AbstractMediator<LobbyServerMediator>
	{
		private static Lobby_Client_Server _clientServer;
		private static Lobby_Region _headConnection;

		private static Lobby_Database _databaseClient;

		public LobbyServerMediator(int ticks = 30) : base(ticks) { }

		public async Task StartServers<T>(T param)
		{
			if (typeof(T) != typeof(string[]))
			{
				return;
			}

			string[] args = (string[])Convert.ChangeType(param, typeof(string[]));

			_clientServer = await StartClientServer();
#if !DEBUG
			_headConnection = await InitConnectionToRegionServer(args);
			_databaseClient = await InitConnectionToDatabaseServer(args);
#endif



		}

		public override void OnUpdate() { }

		public async static Task<Lobby_Client_Server> StartClientServer()
		{
			return await ServerInitialisator<Lobby_Client_Server>.StartServer(27000, 27200);
		}

		public async static Task<Lobby_Region> InitConnectionToRegionServer(string[] args)
		{
			// TCP server address
			string address = "127.0.0.1";
			if (args != null && args.Length > 0)
				address = args[0];


			// TCP server port
			int port = 6380;
			if (args != null && args.Length > 1)
				port = int.Parse(args[1]);

			Lobby_Region headConnection = new Lobby_Region(address, port);

			bool isConnected = headConnection.ConnectAsync();

			if (isConnected == false)
			{
				while (headConnection.IsConnected == false)
					await Task.Yield();
			}

			return headConnection;
		}

		public async static Task<Lobby_Database> InitConnectionToDatabaseServer(string[] args)
		{
			// TCP server address
			string address = "127.0.0.1";
			if (args != null && args.Length > 0)
				address = args[0];


			// TCP server port
			int port = 9090;
			if (args != null && args.Length > 1)
				port = int.Parse(args[1]);

			Lobby_Database headConnection = new Lobby_Database(address, port);

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
