using System.Threading.Tasks;
using LobbyServer.Client;
using LobbyServer.Region;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace LobbyServer
{
	class Program
	{
		private static Lobby_Client_Server _clientServer;
		private static Lobby_Region _headConnection;
		private static LobbyServerMediator _mediator;

		static async Task Main(string[] args)
		{
			Task<Lobby_Client_Server> startClientServerTask = StartClientServer();
			Task<Lobby_Region> initConnectionToFrontendDistributionServerTask = InitConnectionToFrontendDistributionServer(args);

			_clientServer = await startClientServerTask;
			_headConnection = await initConnectionToFrontendDistributionServerTask;

			_mediator = new LobbyServerMediator(40);

			new ConsoleInput<LobbyServerMediator>(_mediator);
		}

		public async static Task<Lobby_Client_Server> StartClientServer()
		{
			return await ServerInitialisator<Lobby_Client_Server>.StartServer(27000, 27200);
		}

		public async static Task<Lobby_Region> InitConnectionToFrontendDistributionServer(string[] args)
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
	}
}
