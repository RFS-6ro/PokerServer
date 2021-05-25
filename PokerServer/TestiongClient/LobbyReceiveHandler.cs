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

	public class LobbyReceiveHandler : IReceivedMessageHandler
	{
		public Dictionary<int, Action<UniCastPacket>> Handlers { get; }

		public LobbyReceiveHandler()
		{
			//Handlers.Add((int)lobbyTOclient., (x) => { });
			//Handlers.Add((int)lobbyTOclient., (x) => { });
			//Handlers.Add((int)lobbyTOclient., (x) => { });
			//Handlers.Add((int)lobbyTOclient., (x) => { });
		}
	}
}