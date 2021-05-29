using System;
using System.Net;
using RegionServer.Handlers;
using UniCastCommonData;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network;

namespace RegionServer
{
	public class Region_FrontendDistribution : AbstractTCPClient<ReceiveFromFrontendDistributionHandler, SendToFrontendDistributionHandler>
	{
		public override ActorType SenderType => ActorType.FrontendDistributionServer;

		public override ActorType ReceiverType => ActorType.RegionServer;

		public Region_FrontendDistribution(IPAddress address, int port) : base(address, port)
		{
		}

		public Region_FrontendDistribution(string address, int port) : base(address, port)
		{
		}
	}
}
