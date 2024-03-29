﻿using System;
using System.Net;
using FrontendDistributionServer.Region.Handlers;
using UniCastCommonData.Network;
using UniCastCommonData.Handlers;

namespace FrontendDistributionServer.Region
{
	public class FrontendDistribution_Region_Server : AbstractTCPServer<
		ReceiveFromRegionHandler,
		SendToRegionHandler,
		FrontendDistribution_Region_Server>
	{
		public override ActorType ServerType => ActorType.FrontendDistributionServer;

		public override ActorType ClientType => ActorType.RegionServer;

		public FrontendDistribution_Region_Server(IPAddress address, int port) : base(address, port)
		{
		}

		protected override TcpSession CreateSession()
		{
			return new FrontendDistribution_Region_Session(this);
		}
	}
}
