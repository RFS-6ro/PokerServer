using System;
using System.Net;
using TestingClient.Handlers;
using UniCastCommonData;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network;

namespace TestingClient
{
	class Program
	{
		static void Main(string[] args)
		{
		}
	}
}

internal sealed class Client : AbstractTCPClient<ReceiveFromLobbyHandler, SendToLobbyHandler, lobbyTOclient, clientTOlobby>
{
	public override ActorType ClientType => throw new NotImplementedException();

	public override ActorType ServerType => throw new NotImplementedException();

	public Client(IPAddress address, int port) : base(address, port)
	{
		SendHandler.Handlers[clientTOlobby.Count].Invoke(null);
	}
}