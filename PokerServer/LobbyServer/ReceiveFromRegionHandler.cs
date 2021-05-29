using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace LobbyServer.Handlers
{
	public enum regionTOlobby
	{
		None = 0,

		Count
	}

	public class ReceiveFromRegionHandler : IReceivedMessageHandler<regionTOlobby>
	{
		public Dictionary<regionTOlobby, Action<UniCastPacket>> Handlers { get; }

		public ReceiveFromRegionHandler()
		{
			//Handlers.Add((int)regionTOlobby., (x) => { });
		}
	}
}