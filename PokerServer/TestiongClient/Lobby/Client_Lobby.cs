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
		public override ActorType ClientType => ActorType.Client;
		public override ActorType ServerType => ActorType.LobbyServer;

		public string Name { get; }

		public Client_Lobby(string name, IPAddress address, int port) : base(address, port)
		{
			Name = name;
		}

		public Client_Lobby(string name, string address, int port) : base(address, port)
		{
			Name = name;
		}

		public Client_Lobby(string name, IPEndPoint endpoint) : base(endpoint)
		{
			Name = name;
		}
	}
}
