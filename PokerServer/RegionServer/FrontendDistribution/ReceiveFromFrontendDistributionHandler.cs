using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace RegionServer.FrontendDistributionSide.Handlers
{
	public enum frontendTOregion
	{
		None = 0,

		Count
	}

	public class ReceiveFromFrontendDistributionHandler : IReceivedMessageHandler<int>
	{
		public Dictionary<int, Action<UniCastPacket>> Handlers { get; } = new Dictionary<int, Action<UniCastPacket>>();

		public ReceiveFromFrontendDistributionHandler()
		{
			//Handlers.Add((int)frontendTOregion., (x) => { });
		}
	}
}