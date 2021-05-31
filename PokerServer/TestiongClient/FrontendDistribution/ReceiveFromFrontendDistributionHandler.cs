using System;
using System.Collections.Generic;
using System.Text;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace TestingClient.FrontendDistributionSide.Handlers
{
	public enum frontendTOclient
	{
		None = 0,

		Count
	}

	public class ReceiveFromFrontendDistributionHandler : IReceivedMessageHandler<int>
	{
		public Dictionary<int, Action<UniCastPacket>> Handlers { get; } = new Dictionary<int, Action<UniCastPacket>>();

		public ReceiveFromFrontendDistributionHandler()
		{
			Handlers.Add((int)frontendTOclient.Count, Test);
		}

		private void Test(UniCastPacket packet)
		{
		}
	}
}