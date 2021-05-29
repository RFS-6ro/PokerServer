using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace RegionServer.Handlers
{
	public enum frontendTOregion
	{
		None = 0,

		Count
	}

	public class ReceiveFromFrontendDistributionHandler : IReceivedMessageHandler<frontendTOregion>
	{
		public Dictionary<frontendTOregion, Action<UniCastPacket>> Handlers { get; }

		public ReceiveFromFrontendDistributionHandler()
		{
			//Handlers.Add((int)frontendTOregion., (x) => { });
		}
	}
}