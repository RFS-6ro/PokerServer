using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace LobbyServer.Handlers
{
	public enum clientTOlobby
	{
		None = 0,

		Count
	}

	public class ReceiveFromClientHandler : IReceivedMessageHandler<clientTOlobby>
	{
		public Dictionary<clientTOlobby, Action<UniCastPacket>> Handlers { get; }

		public ReceiveFromClientHandler()
		{
			//Handlers.Add((int)clientTOlobby., (x) => { });
		}
	}
}
