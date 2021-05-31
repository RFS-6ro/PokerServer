using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace FrontendDistributionServer.RegionSide.Handlers
{
	public enum regionTOfrontend
	{
		None = 0,

		Count
	}

	public class ReceiveFromRegionHandler : IReceivedMessageHandler<int>
	{
		public Dictionary<int, Action<UniCastPacket>> Handlers { get; } = new Dictionary<int, Action<UniCastPacket>>();

		public ReceiveFromRegionHandler()
		{
			//Handlers.Add((int)regionTOfrontend., (x) => { });
		}
	}
}
