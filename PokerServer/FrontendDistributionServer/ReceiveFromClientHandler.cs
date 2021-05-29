using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace FrontendDistributionServer.Handlers
{
	public enum clientTOfrontend
	{
		None = 0,

		Count
	}

	public class ReceiveFromClientHandler : IReceivedMessageHandler<clientTOfrontend>
	{
		public Dictionary<clientTOfrontend, Action<UniCastPacket>> Handlers { get; }

		public ReceiveFromClientHandler()
		{
			//Handlers.Add((int)clientTOfrontend., (x) => { });
		}
	}
}
