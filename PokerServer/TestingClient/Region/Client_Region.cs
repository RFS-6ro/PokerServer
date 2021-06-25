using System;
using System.Net;
using UniCastCommonData.Network;
using TestingClient.Region.Handlers;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace TestingClient.Region
{
	public class Client_Region : AbstractTCPClient<
		ReceiveFromRegionHandler,
		SendToRegionHandler,
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

		public Client_Region(IPEndPoint endpoint) : base(endpoint)
		{
		}
	}
}
