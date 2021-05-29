using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace TestingClient.Handlers
{
	public enum frontendTOclient
	{
		None = 0,

		Count
	}

	public class ReceiveFromFrontendDistributionHandler : IReceivedMessageHandler<frontendTOclient>
	{
		public Dictionary<frontendTOclient, Action<UniCastPacket>> Handlers { get; }

		public ReceiveFromFrontendDistributionHandler()
		{
			//Handlers.Add((int)frontendTOclient., (x) => { });
		}
	}
}