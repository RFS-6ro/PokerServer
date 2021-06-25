using ServerPool;

namespace FrontendDistributionServer.Region
{
	internal class RegionServerProcessFactory : IServerProcessFactory<Region_Server_Process, string[]>
	{
		public Region_Server_Process CreateWithParams(string[] param)
		{
			return new Region_Server_Process(param);
		}
	}
}
