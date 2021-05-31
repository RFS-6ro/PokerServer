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

	public class ReceiveFromLobbyHandler : IReceivedMessageHandler<int>
	{
		public Dictionary<int, Action<UniCastPacket>> Handlers { get; } = new Dictionary<int, Action<UniCastPacket>>();

		public ReceiveFromLobbyHandler()
		{
			//Handlers.Add((int)lobbyTOclient., (x) => { });
		}
	}
}