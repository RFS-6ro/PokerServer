using System;
using System.Net;
using UniCastCommonData.Network;
using TestingClient.Handlers;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace TestingClient
{
	public class Client_Lobby : AbstractTCPClient<ReceiveFromLobbyHandler, lobbyTOclient, SendToLobbyHandler, clientTOlobby>
	{
		public override ActorType SenderType => ActorType.LobbyServer;

		public override ActorType ReceiverType => ActorType.Client;

		public Client_Lobby(IPAddress address, int port) : base(address, port)
		{
		}

		public Client_Lobby(string address, int port) : base(address, port)
		{
		}
	}
}
