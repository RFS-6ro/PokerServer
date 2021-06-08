
using System;
using System.Net;
using LobbyServer.Client.Handlers;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network;

namespace LobbyServer.Client
{
	public class Lobby_Client_Server : AbstractTCPServer<
		ReceiveFromClientHandler,
		SendToClientHandler,
		Lobby_Client_Server>
	{
		public override ActorType ServerType => ActorType.LobbyServer;

		public override ActorType ClientType => ActorType.Client;

		public Lobby_Client_Server(IPAddress address, int port) : base(address, port)
		{
		}

		protected override TcpSession CreateSession()
		{
			return new Lobby_Client_Session(this);
		}
	}
}
