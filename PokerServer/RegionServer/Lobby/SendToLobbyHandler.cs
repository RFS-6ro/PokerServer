using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace RegionServer.LobbySide.Handlers
{
	public enum regionTOlobby
	{
		None = 0,

		Count
	}

	public class SendToLobbyHandler : ISendMessageHandler<int>
	{
		public Dictionary<int, Action<InitialSendingData>> Handlers { get; } = new Dictionary<int, Action<InitialSendingData>>();

		public SendToLobbyHandler()
		{
			//Handlers.Add((int)regionTOlobby., (x) => { });
		}
	}
}