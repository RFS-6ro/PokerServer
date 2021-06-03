using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network.MessageHandlers;

namespace FrontendDistributionServer.Region.Handlers
{
	public enum frontendTOregion
	{
		None = 0,

		Count,


		Test
	}

	public class SendToRegionHandler : SessionSender<FrontendDistribution_Region_Server>
	{
		public SendToRegionHandler()
		{
			Handlers.Add((int)frontendTOregion.Test, Test);
		}

		private void Test(InitialSendingData data)
		{
			using (UniCastPacket packet = new UniCastPacket(data))
			{
				packet.Write(GetType().ToString());

				Sender.SendAsync(packet);
			}
		}
	}
}
