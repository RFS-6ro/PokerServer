using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace FrontendDistributionServer.ClientSide.Handlers
{
	public enum clientTOfrontend
	{
		None = 0,

		Count
	}

	public class ReceiveFromClientHandler : IReceivedMessageHandler<int>
	{
		public Dictionary<int, Action<UniCastPacket>> Handlers { get; } = new Dictionary<int, Action<UniCastPacket>>();

		public ReceiveFromClientHandler()
		{
			Handlers.Add((int)clientTOfrontend.Count, Test);
		}

		private void Test(UniCastPacket packet)
		{
		}
	}
}
