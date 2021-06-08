using TestingClient.FrontendDistribution;
using System.Net;
using TestingClient.Region;
using TestingClient.Lobby;
using System.Threading.Tasks;
using UniCastCommonData.Handlers;

namespace TestingClient
{
	public class TestingClientMediator : AbstractMediator<TestingClientMediator>
	{
		private Client_FrontendDistributor _headConnection;

		private Client_Region _regionConnection;
		private Client_Lobby _lobbyConnection;

		private string _name = null;

		public TestingClientMediator(int ticks = 30) : base(ticks) { }

		public async override Task StartServers()
		{
			while (_name == null)
			{
				await Task.Yield();
			}

#if DEBUG
			_lobbyConnection = await InitConnectionToLobbyServer(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 27000));
			return;
#endif
			_headConnection = await InitConnectionToFrontendDistributionServer();
		}

		public void SetName(string name)
		{
			_name = name;
		}

		public override void OnUpdate() { }

		public async Task<Client_FrontendDistributor> InitConnectionToFrontendDistributionServer()
		{
			// TCP server address
			string address = "127.0.0.1";

			// TCP server port
			int port = 5555;

			Client_FrontendDistributor headConnection = new Client_FrontendDistributor(address, port);

			bool isConnected = headConnection.ConnectAsync();

			if (isConnected == false)
			{
				while (headConnection.IsConnected == false)
					await Task.Yield();
			}

			return headConnection;
		}

		public async Task<Client_Region> InitConnectionToRegionServer(IPEndPoint ip)
		{
			Client_Region regionConnection = new Client_Region(ip);

			bool isConnected = regionConnection.ConnectAsync();

			if (isConnected == false)
			{
				while (regionConnection.IsConnected == false)
					await Task.Yield();
			}

			return regionConnection;
		}

		public async Task<Client_Lobby> InitConnectionToLobbyServer(IPEndPoint ip)
		{
			Client_Lobby lobbyConnection = new Client_Lobby(_name, ip);

			bool isConnected = lobbyConnection.ConnectAsync();

			if (isConnected == false)
			{
				while (lobbyConnection.IsConnected == false)
					await Task.Yield();
			}

			return lobbyConnection;
		}
	}
}
