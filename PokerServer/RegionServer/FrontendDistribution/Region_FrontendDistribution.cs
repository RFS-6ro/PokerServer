using System.Net;
using RegionServer.FrontendDistribution.Handlers;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network;

namespace RegionServer.FrontendDistribution
{
	public class Region_FrontendDistribution : AbstractTCPClient<
		ReceiveFromFrontendDistributionHandler, int,
		SendToFrontendDistributionHandler, int,
		Region_FrontendDistribution>
	{
		public override ActorType ServerType => ActorType.FrontendDistributionServer;

		public override ActorType ClientType => ActorType.Client;

		public Region_FrontendDistribution(IPAddress address, int port) : base(address, port)
		{
		}

		public Region_FrontendDistribution(string address, int port) : base(address, port)
		{
		}
	}
}
