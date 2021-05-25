using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace TestingClient.Handlers
{
	public enum clientTOlobby
	{
		None = 0,

		Count
	}

	public class SendToLobbyHandler : ISendMessageHandler
	{
		public Dictionary<int, Action<UniCastPacket>> Handlers { get; }

		public SendToLobbyHandler()
		{
			//Handlers.Add((int)clientTOlobby., (x) => { });
			//Handlers.Add((int)clientTOlobby., (x) => { });
			//Handlers.Add((int)clientTOlobby., (x) => { });
			//Handlers.Add((int)clientTOlobby., (x) => { });
		}
	}
}