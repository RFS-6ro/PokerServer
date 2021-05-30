using System;
using System.Net;
using FrontendDistributionServer.Handlers;
using UniCastCommonData.Network;
using UniCastCommonData.Handlers;

namespace FrontendDistributionServer
{
	public class FrontendDistribution_Client_Server : AbstractTCPServer<
			ClientSessionBuilder,
			ReceiveFromClientHandler,
			clientTOfrontend,
			SendToClientHandler,
			frontendTOclient,
			FrontendDistribution_Client_Server>
	{
		public override ActorType ServerType => ActorType.FrontendDistributionServer;

		public override ActorType ClientType => ActorType.Client;

		public FrontendDistribution_Client_Server(IPAddress address, int port) : base(address, port)
		{
		}
	}
}