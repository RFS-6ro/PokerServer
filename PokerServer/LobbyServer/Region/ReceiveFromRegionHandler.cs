using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace LobbyServer.RegionSide.Handlers
{
	public enum regionTOlobby
	{
		None = 0,

		Count
	}

	public class ReceiveFromRegionHandler : IReceivedMessageHandler<int>
	{
		public Dictionary<int, Action<UniCastPacket>> Handlers { get; } = new Dictionary<int, Action<UniCastPacket>>();

		public ReceiveFromRegionHandler()
		{
			//Handlers.Add((int)regionTOlobby., (x) => { });
		}
	}
}