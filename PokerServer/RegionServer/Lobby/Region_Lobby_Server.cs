using System;
using System.Net;
using RegionServer.Lobby.Handlers;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network;

namespace RegionServer.Lobby
{
	public class Region_Lobby_Server : AbstractTCPServer<
		ReceiveFromLobbyHandler,
		SendToLobbyHandler,
		Region_Lobby_Server>
	{
		public override ActorType ServerType => ActorType.RegionServer;

		public override ActorType ClientType => ActorType.LobbyServer;

		public Region_Lobby_Server(IPAddress address, int port) : base(address, port)
		{
		}

		protected override TcpSession CreateSession()
		{
			return new Region_Lobby_Session(this);
		}
	}
}
