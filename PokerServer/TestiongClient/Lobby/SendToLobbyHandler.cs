using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace TestingClient.LobbySide.Handlers
{
	public enum clientTOlobby
	{
		None = 0,

		Count
	}

	public class SendToLobbyHandler : ISendMessageHandler<int>
	{
		public Dictionary<int, Action<InitialSendingData>> Handlers { get; } = new Dictionary<int, Action<InitialSendingData>>();

		public SendToLobbyHandler()
		{
			//Handlers.Add((int)clientTOlobby., (x) => { });
		}
	}
}