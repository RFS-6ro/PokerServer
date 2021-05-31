
using System;
using System.Net;
using LobbyServer.ClientSide.Handlers;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network;

namespace LobbyServer.ClientSide
{
	public class Lobby_Client_Server : AbstractTCPServer<
		ClientSessionBuilder,
		ReceiveFromClientHandler, int,
		SendToClientHandler, int,
		Lobby_Client_Server>
	{
		public override ActorType ServerType => ActorType.LobbyServer;

		public override ActorType ClientType => ActorType.Client;

		public Lobby_Client_Server(IPAddress address, int port) : base(address, port)
		{
		}
	}
}
