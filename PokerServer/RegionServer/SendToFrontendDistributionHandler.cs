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

	public class SendToFrontendDistributionHandler : ISendMessageHandler<regionTOfrontend>
	{
		public Dictionary<regionTOfrontend, Action<UniCastPacket>> Handlers { get; }

		public SendToFrontendDistributionHandler()
		{
			//Handlers.Add((int)regionTOfrontend., (x) => { });
		}
	}
}
