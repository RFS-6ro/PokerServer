
using System;
using System.Net;
using LobbyServer.Handlers;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network;

namespace LobbyServer
{
	public class Lobby_Client_Server : AbstractTCPServer<ClientSessionBuilder, ReceiveFromClientHandler, clientTOlobby, SendToClientHandler, lobbyTOclient>
	{
		public override ActorType ServerType => ActorType.LobbyServer;

		public override ActorType ClientType => ActorType.Client;

		public Lobby_Client_Server(IPAddress address, int port) : base(address, port)
		{
		}
	}
}
