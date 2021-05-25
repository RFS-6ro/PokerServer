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

	public class FrontendDistributionReceiveHandler : IReceivedMessageHandler
	{
		public Dictionary<int, Action<UniCastPacket>> Handlers { get; }

		public FrontendDistributionReceiveHandler()
		{
			//Handlers.Add((int)frontendTOclient., (x) => { });
			//Handlers.Add((int)frontendTOclient., (x) => { });
			//Handlers.Add((int)frontendTOclient., (x) => { });
			//Handlers.Add((int)frontendTOclient., (x) => { });
		}
	}
}