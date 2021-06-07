using System;
using UniCastCommonData;
using UniCastCommonData.Network.MessageHandlers;
using UniCastCommonData.Packet.InitialDatas;

namespace FrontendDistributionServer.Region.Handlers
{
	public enum frontendTOregion
	{
		None = 0,

		Connect,
		Disconnect,

		Count,


		Test
	}

	public class SendToRegionHandler : SessionSender<FrontendDistribution_Region_Server>
	{
		public SendToRegionHandler()
		{
			Handlers.Add((int)frontendTOregion.Test, Test);
			Handlers.Add((int)frontendTOregion.Connect, Connect);
			Handlers.Add((int)frontendTOregion.Disconnect, Disconnect);
		}

		private void Disconnect(InitialSendingData data)
		{
			using (UniCastPacket packet = new UniCastPacket(data))
			{


				Sender.SendAsync(packet);
			}
		}

		private void Connect(InitialSendingData data)
		{
			using (UniCastPacket packet = new UniCastPacket(data))
			{


				Sender.SendAsync(packet);
			}
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
