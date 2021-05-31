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

	public class ReceiveFromFrontendDistributionHandler : IReceivedMessageHandler<InitialSendingData>
	{
		public Dictionary<InitialSendingData, Action<UniCastPacket>> Handlers { get; } = new Dictionary<InitialSendingData, Action<UniCastPacket>>();

		public ReceiveFromFrontendDistributionHandler()
		{
			//Handlers.Add((int)frontendTOregion., (x) => { });
		}
	}
}