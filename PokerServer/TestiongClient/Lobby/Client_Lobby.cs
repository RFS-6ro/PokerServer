using System;
using System.Net;
using UniCastCommonData.Network;
using TestingClient.Lobby.Handlers;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace TestingClient.Lobby
{
	public class Client_Lobby : AbstractTCPClient<
		ReceiveFromLobbyHandler,
		SendToLobbyHandler,
		Client_Lobby>
	{
		public override ActorType ClientType => ActorType.LobbyServer;

		public override ActorType ServerType => ActorType.Client;

		public Client_Lobby(IPAddress address, int port) : base(address, port)
		{
		}

		public Client_Lobby(string address, int port) : base(address, port)
		{
		}

		public Client_Lobby(IPEndPoint endpoint) : base(endpoint)
		{
		}
	}
}
