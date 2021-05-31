using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace LobbyServer.ClientSide.Handlers
{
	public enum lobbyTOclient
	{
		None = 0,

		Count
	}

	public class SendToClientHandler : ISendMessageHandler<int>
	{
		public Dictionary<int, Action<InitialSendingData>> Handlers { get; } = new Dictionary<int, Action<InitialSendingData>>();

		public SendToClientHandler()
		{
			//Handlers.Add((int)lobbyTOclient., (x) => { });
		}
	}
}
