using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace RegionServer.Handlers
{
	public enum regionTOfrontend
	{
		None = 0,

		Count
	}

	public class SendToFrontendDistributionHandler : ISendMessageHandler<int>
	{
		public Dictionary<int, Action<InitialSendingData>> Handlers { get; } = new Dictionary<int, Action<InitialSendingData>>();

		public SendToFrontendDistributionHandler()
		{
			//Handlers.Add((int)regionTOfrontend., (x) => { });
		}
	}
}
