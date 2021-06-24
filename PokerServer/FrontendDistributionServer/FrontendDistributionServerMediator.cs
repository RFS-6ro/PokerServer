using System;
using System.Threading.Tasks;
using FrontendDistributionServer.Client;
using FrontendDistributionServer.Database;
using FrontendDistributionServer.Region;
using ServerPool;
using UniCastCommonData.Handlers;

namespace FrontendDistributionServer
{
	public class FrontendDistributionServerMediator : AbstractMediator<FrontendDistributionServerMediator>
	{
		private static FrontendDistribution_Region_Server _regionServer;
		private static FrontendDistribution_Client_Server _clientServer;

		private static ServerPool<Region_Server_Process> _regions;

		private static FrontendDistribution_Database _databaseClient;

		public FrontendDistributionServerMediator(int ticks = 30) : base(ticks) { }

		public async Task StartServers<T>(T param)
		{
			if (typeof(T) != typeof(string[]))
			{
				return;
			}

			string[] args = (string[])Convert.ChangeType(param, typeof(string[]));

			Task<FrontendDistribution_Region_Server> startRegionServerTask = StartRegionServer();
			Task<FrontendDistribution_Client_Server> startClientServerTask = StartClientServer();

			_clientServer = await startClientServerTask;
			_regionServer = await startRegionServerTask;


			var factory = new RegionServerProcessFactory();

#if false
			_regions = new ServerPool<Region_Server_Process>(() => factory.CreateWithParams(args));
			await Task.WhenAll(
				new Task[] {
								_regions.CreateNew(),
								_regions.CreateNew(),
								_regions.CreateNew()
				});
#endif

			_databaseClient = await InitConnectionToDatabaseServer(args);
		}

		public async static Task<FrontendDistribution_Region_Server> StartRegionServer()
		{
			return await ServerInitialisator<FrontendDistribution_Region_Server>.StartServer(1111);
		}

		public async static Task<FrontendDistribution_Client_Server> StartClientServer()
		{
			return await ServerInitialisator<FrontendDistribution_Client_Server>.StartServer(5555);
		}

		public async static Task<FrontendDistribution_Database> InitConnectionToDatabaseServer(string[] args)
		{
			// TCP server address
			string address = "127.0.0.1";
			if (args != null && args.Length > 0)
				address = args[0];


			// TCP server port
			int port = 9090;
			if (args != null && args.Length > 1)
				port = int.Parse(args[1]);

			FrontendDistribution_Database headConnection = new FrontendDistribution_Database(address, port);

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
