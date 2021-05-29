using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace FrontendDistributionServer.Handlers
{
	public enum regionTOfrontend
	{
		None = 0,

		Count
	}

	public class ReceiveFromRegionHandler : IReceivedMessageHandler<regionTOfrontend>
	{
		public Dictionary<regionTOfrontend, Action<UniCastPacket>> Handlers { get; }

		public ReceiveFromRegionHandler()
		{
			//Handlers.Add((int)regionTOfrontend., (x) => { });
		}
	}
}
