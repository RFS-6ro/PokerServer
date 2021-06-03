﻿using TestingClient.FrontendDistribution;
using System.Net;
using TestingClient.Region;
using TestingClient.Lobby;
using System.Threading.Tasks;
using UniCastCommonData;

namespace TestingClient
{
	class Program
	{
		private static Client_FrontendDistributor _headConnection;
		private static Client_Region _regionConnection;
		private static Client_Lobby _lobbyConnection;
		private static TestingClientMediator _mediator;

		static async Task Main(string[] args)
		{
			Task<Client_FrontendDistributor> initConnectionToFrontendDistributionServer = InitConnectionToFrontendDistributionServer();
			_headConnection = await initConnectionToFrontendDistributionServer;

			_mediator = new TestingClientMediator(60);

			new ConsoleInput<TestingClientMediator>(_mediator);
		}

		public async static Task<Client_FrontendDistributor> InitConnectionToFrontendDistributionServer()
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

		public async static Task<Client_Region> InitConnectionToRegionServer(IPEndPoint ip)
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

		public async static Task<Client_Lobby> InitConnectionToLobbyServer(IPEndPoint ip)
		{
			Client_Lobby lobbyConnection = new Client_Lobby(ip);

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
