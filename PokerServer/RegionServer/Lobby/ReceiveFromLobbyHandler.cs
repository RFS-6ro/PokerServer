using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace RegionServer.Lobby.Handlers
{
	public enum lobbyTOregion
	{
		None = 0,

		Count
	}

	public class ReceiveFromLobbyHandler : IReceivedMessageHandler<int>
	{
		public Dictionary<int, Action<UniCastPacket>> Handlers { get; } = new Dictionary<int, Action<UniCastPacket>>();

		public ReceiveFromLobbyHandler()
		{
			//Handlers.Add((int)lobbyTOregion., (x) => { });
		}
	}
}