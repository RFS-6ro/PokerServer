using System;
using System.Collections.Generic;
using System.Text;
using UniCastCommonData;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network.MessageHandlers;

namespace FrontendDistributionServer.Client.Handlers
{
	public enum frontendTOclient
	{
		None = 0,

		Count,


		Test
	}

	public class SendToClientHandler : SessionSender<FrontendDistribution_Client_Server>
	{
		public SendToClientHandler()
		{
			Handlers.Add((int)frontendTOclient.Test, Test);
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
