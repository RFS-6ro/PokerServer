using System;
using System.Net;
using UniCastCommonData.Network;
using TestingClient.Handlers;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace TestingClient
{
	public class Client_FrontendDistributor : AbstractTCPClient<ReceiveFromFrontendDistributionHandler, SendToFrontendDistributionHandler>
	{
		public override ActorType SenderType => ActorType.FrontendDistributionServer;

		public override ActorType ReceiverType => ActorType.Client;

		public Client_FrontendDistributor(IPAddress address, int port) : base(address, port)
		{
		}

		public Client_FrontendDistributor(string address, int port) : base(address, port)
		{
		}
	}
}
