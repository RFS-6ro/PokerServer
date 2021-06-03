using System;
using System.Threading;
using System.Threading.Tasks;
using FrontendDistributionServer.Client;
using FrontendDistributionServer.Region;
using UniCastCommonData;
using UniCastCommonData.Handlers;
using UniCastCommonData.ServerPool;

namespace FrontendDistributionServer
{
	class Program
	{
		private static FrontendDistribution_Region_Server _regionServer;
		private static FrontendDistribution_Client_Server _clientServer;
#if !DEBUG
		private static ServerPool<Region_Server_Process> _regions;
#endif

		private static FrontendDistributionServerMediator _mediator;

		static async Task Main(string[] args)
		{
			Task<FrontendDistribution_Region_Server> startRegionServerTask = StartRegionServer();
			Task<FrontendDistribution_Client_Server> startClientServerTask = StartClientServer();

			_clientServer = await startClientServerTask;
			_regionServer = await startRegionServerTask;

#if !DEBUG
			var factory = new RegionServerProcessFactory();

			_regions = new ServerPool<Region_Server_Process>(() => factory.CreateWithParams(args));
			await Task.WhenAll(
				new Task[] {
								_regions.CreateNew(),
								_regions.CreateNew(),
								_regions.CreateNew()
				});
#endif

			_mediator = new FrontendDistributionServerMediator(20);

			new ConsoleInput<FrontendDistributionServerMediator>(_mediator);
		}

		public async static Task<FrontendDistribution_Region_Server> StartRegionServer()
		{
			return await ServerInitialisator<FrontendDistribution_Region_Server>.StartServer(1111);
		}

		public async static Task<FrontendDistribution_Client_Server> StartClientServer()
		{
			return await ServerInitialisator<FrontendDistribution_Client_Server>.StartServer(5555);
		}
	}
}

