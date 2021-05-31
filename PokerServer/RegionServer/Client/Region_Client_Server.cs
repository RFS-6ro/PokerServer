using System;
using System.Net;
using UniCastCommonData.Network;
using RegionServer.ClientSide.Handlers;
using UniCastCommonData.Handlers;

namespace RegionServer.ClientSide
{
	public class Region_Client_Server : AbstractTCPServer<
		ClientSessionBuilder,
		ReceiveFromClientHandler, int,
		SendToClientHandler, int,
		Region_Client_Server>
	{
		public override ActorType ServerType => ActorType.RegionServer;

		public override ActorType ClientType => ActorType.Client;

		public Region_Client_Server(IPAddress address, int port) : base(address, port)
		{
		}
	}
}
