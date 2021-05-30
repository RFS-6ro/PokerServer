using System;
using System.Net;
using FrontendDistributionServer.Handlers;
using UniCastCommonData.Network;
using UniCastCommonData.Handlers;

namespace FrontendDistributionServer
{
	public class FrontendDistribution_Region_Server : AbstractTCPServer<RegionSessionBuilder, ReceiveFromRegionHandler, regionTOfrontend, SendToRegionHandler, frontendTOregion>
	{
		public override ActorType ServerType => ActorType.FrontendDistributionServer;

		public override ActorType ClientType => ActorType.RegionServer;

		public FrontendDistribution_Region_Server(IPAddress address, int port) : base(address, port)
		{
		}
	}
}
