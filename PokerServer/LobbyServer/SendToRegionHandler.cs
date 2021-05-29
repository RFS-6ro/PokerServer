using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace LobbyServer.Handlers
{
	public enum lobbyTOregion
	{
		None = 0,

		Count
	}

	public class SendToRegionHandler : ISendMessageHandler<lobbyTOregion>
	{
		public Dictionary<lobbyTOregion, Action<UniCastPacket>> Handlers { get; }

		public SendToRegionHandler()
		{
			//Handlers.Add(lobbyTOregion., (x) => { });
		}
	}
}