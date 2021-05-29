using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace TestingClient.Handlers
{
	public enum lobbyTOclient
	{
		None = 0,

		Count
	}

	public class ReceiveFromLobbyHandler : IReceivedMessageHandler<lobbyTOclient>
	{
		public Dictionary<lobbyTOclient, Action<UniCastPacket>> Handlers { get; }

		public ReceiveFromLobbyHandler()
		{
			//Handlers.Add((int)lobbyTOclient., (x) => { });
		}
	}
}