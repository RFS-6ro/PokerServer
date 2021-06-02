using System.Threading.Tasks;
using FrontendDistributionServer.Client;
using FrontendDistributionServer.Region;
using UniCastCommonData.Handlers;

namespace FrontendDistributionServer
{
	class Program
	{
		private static FrontendDistribution_Region_Server _regionServer;
		private static FrontendDistribution_Client_Server _clientServer;

		static async void Main(string[] args)
		{
			Task<FrontendDistribution_Region_Server> startRegionServerTask = StartRegionServer();
			Task<FrontendDistribution_Client_Server> startClientServerTask = StartClientServer();

			_clientServer = await startClientServerTask;
			_regionServer = await startRegionServerTask;
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
