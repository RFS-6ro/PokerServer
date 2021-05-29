using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace FrontendDistributionServer.Handlers
{
	public enum frontendTOclient
	{
		None = 0,

		Count
	}

	public class SendToClientHandler : ISendMessageHandler<frontendTOclient>
	{
		public Dictionary<frontendTOclient, Action<UniCastPacket>> Handlers { get; }

		public SendToClientHandler()
		{
			//Handlers.Add((int)frontendTOclient., (x) => { });
		}
	}
}
