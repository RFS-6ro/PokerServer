using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace LobbyServer.RegionSide.Handlers
{
	public enum lobbyTOregion
	{
		None = 0,

		Count
	}

	public class SendToRegionHandler : ISendMessageHandler<int>
	{
		public Dictionary<int, Action<InitialSendingData>> Handlers { get; } = new Dictionary<int, Action<InitialSendingData>>();

		public SendToRegionHandler()
		{
			//Handlers.Add(lobbyTOregion., (x) => { });
		}
	}
}