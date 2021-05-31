using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace RegionServer.ClientSide.Handlers
{
	public enum clientTOregion
	{
		None = 0,

		Count
	}

	public class ReceiveFromClientHandler : IReceivedMessageHandler<int>
	{
		public Dictionary<int, Action<UniCastPacket>> Handlers { get; } = new Dictionary<int, Action<UniCastPacket>>();

		public ReceiveFromClientHandler()
		{
			//Handlers.Add((int)clientTOregion., (x) => { });
		}
	}
}