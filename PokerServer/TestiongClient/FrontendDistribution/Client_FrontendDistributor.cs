using System;
using System.Net;
using UniCastCommonData.Network;
using TestingClient.FrontendDistributionSide.Handlers;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace TestingClient.FrontendDistributionSide
{
	public class Client_FrontendDistributor : AbstractTCPClient<
		ReceiveFromFrontendDistributionHandler, int,
		SendToFrontendDistributionHandler, int,
		Client_FrontendDistributor>
	{
		public override ActorType ServerType => ActorType.FrontendDistributionServer;

		public override ActorType ClientType => ActorType.Client;

		public Client_FrontendDistributor(IPAddress address, int port) : base(address, port)
		{
		}

		public Client_FrontendDistributor(string address, int port) : base(address, port)
		{
		}
	}
}
