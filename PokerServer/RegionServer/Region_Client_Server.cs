using System;
using System.Net;
using UniCastCommonData.Network;
using RegionServer.Handlers;
using UniCastCommonData.Handlers;

namespace RegionServer
{
	public class Region_Client_Server : AbstractTCPServer<ClientSessionBuilder, ReceiveFromClientHandler, clientTOregion, SendToClientHandler, regionTOclient>
	{
		public override ActorType ServerType => ActorType.RegionServer;

		public override ActorType ClientType => ActorType.Client;

		public Region_Client_Server(IPAddress address, int port) : base(address, port)
		{
		}
	}
}
