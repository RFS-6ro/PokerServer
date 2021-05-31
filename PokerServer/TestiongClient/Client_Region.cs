using System;
using System.Net;
using UniCastCommonData.Network;
using TestingClient.Handlers;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace TestingClient
{
	public class Client_Region : AbstractTCPClient<
		ReceiveFromRegionHandler, int,
		SendToRegionHandler, int,
		Client_Region>
	{
		public override ActorType ClientType => ActorType.RegionServer;

		public override ActorType ServerType => ActorType.Client;

		public Client_Region(IPAddress address, int port) : base(address, port)
		{

		}

		public Client_Region(string address, int port) : base(address, port)
		{
		}
	}
}
