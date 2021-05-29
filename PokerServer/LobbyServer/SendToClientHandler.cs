using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace LobbyServer.Handlers
{
	public enum lobbyTOclient
	{
		None = 0,

		Count
	}

	public class SendToClientHandler : ISendMessageHandler<lobbyTOclient>
	{
		public Dictionary<lobbyTOclient, Action<UniCastPacket>> Handlers { get; }

		public SendToClientHandler()
		{
			//Handlers.Add((int)lobbyTOclient., (x) => { });
		}
	}
}
