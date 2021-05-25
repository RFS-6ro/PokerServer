using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace TestingClient.Handlers
{
	public enum clientTOfrontend
	{
		None = 0,

		Count
	}

	public class SendToFrontendDistributionHandler : ISendMessageHandler
	{
		public Dictionary<int, Action<UniCastPacket>> Handlers { get; }

		public SendToFrontendDistributionHandler()
		{
			//Handlers.Add((int)clientTOfrontend., (x) => { });
			//Handlers.Add((int)clientTOfrontend., (x) => { });
			//Handlers.Add((int)clientTOfrontend., (x) => { });
			//Handlers.Add((int)clientTOfrontend., (x) => { });
		}
	}
}