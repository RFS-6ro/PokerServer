using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace RegionServer.Handlers
{
	public enum clientTOregion
	{
		None = 0,

		Count
	}

	public class ReceiveFromClientHandler : IReceivedMessageHandler<clientTOregion>
	{
		public Dictionary<clientTOregion, Action<UniCastPacket>> Handlers { get; }

		public ReceiveFromClientHandler()
		{
			//Handlers.Add((int)clientTOregion., (x) => { });
		}
	}
}