using System;
using UniCastCommonData;
using UniCastCommonData.Network.MessageHandlers;
using UniCastCommonData.Packet.InitialDatas;

namespace FrontendDistributionServer.Client.Handlers
{
	public enum frontendTOclient
	{
		None = 0,

		Connect,
		Disconnect,

		Count,


		Test
	}

	public class SendToClientHandler : SessionSender<FrontendDistribution_Client_Server>
	{
		public SendToClientHandler()
		{
			Handlers.Add((int)frontendTOclient.Test, Test);
			Handlers.Add((int)frontendTOclient.Connect, Connect);
			Handlers.Add((int)frontendTOclient.Disconnect, Disconnect);
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
