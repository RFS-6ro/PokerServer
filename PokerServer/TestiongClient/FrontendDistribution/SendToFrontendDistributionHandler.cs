using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;

namespace TestingClient.FrontendDistributionSide.Handlers
{
	public enum clientTOfrontend
	{
		None = 0,

		Count
	}

	public class SendToFrontendDistributionHandler : ISendMessageHandler<int>
	{
		public Dictionary<int, Action<InitialSendingData>> Handlers { get; } = new Dictionary<int, Action<InitialSendingData>>();

		public SendToFrontendDistributionHandler()
		{
			Handlers.Add((int)clientTOfrontend.None, Test);
		}

		private void Test(InitialSendingData data)
		{
			using (UniCastPacket packet = new UniCastPacket(Client_FrontendDistributor.Instance.ClientType))
			{
			}
		}
	}
}