using System;
using System.Net;
using UniCastCommonData.Network;
using TestingClient.Handlers;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace TestingClient
{
	public class Client_Region : AbstractTCPClient<ReceiveFromRegionHandler, regionTOclient, SendToRegionHandler, clientTOregion>
	{
		public override ActorType SenderType => ActorType.RegionServer;

		public override ActorType ReceiverType => ActorType.Client;

		public Client_Region(IPAddress address, int port) : base(address, port)
		{
		}

		public Client_Region(string address, int port) : base(address, port)
		{
		}
	}
}
