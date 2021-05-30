using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace RegionServer.Handlers
{
	public enum lobbyTOregion
	{
		None = 0,

		Count
	}

	public class ReceiveFromLobbyHandler : IReceivedMessageHandler<lobbyTOregion>
	{
		public Dictionary<lobbyTOregion, Action<UniCastPacket>> Handlers { get; }

		public ReceiveFromLobbyHandler()
		{
			//Handlers.Add((int)lobbyTOregion., (x) => { });
		}
	}
}