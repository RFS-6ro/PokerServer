using System;
using System.Net;
using FrontendDistributionServer.Client.Handlers;
using UniCastCommonData.Network;
using UniCastCommonData.Handlers;

namespace FrontendDistributionServer.Client
{
	public class FrontendDistribution_Client_Server : AbstractTCPServer<
		ReceiveFromClientHandler,
		SendToClientHandler,
		FrontendDistribution_Client_Server>
	{
		public override ActorType ServerType => ActorType.FrontendDistributionServer;

		public override ActorType ClientType => ActorType.Client;

		public FrontendDistribution_Client_Server(IPAddress address, int port) : base(address, port)
		{
		}

		protected override TcpSession CreateSession()
		{
			return new FrontendDistribution_Client_Session(this);
		}
	}
}