using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network.MessageHandlers;

namespace RegionServer.Client.Handlers
{
	public enum regionTOclient
	{
		None = 0,

		Count,


		Test
	}

	public class SendToClientHandler : SessionSender<Region_Client_Server>
	{
		public SendToClientHandler()
		{
			Handlers.Add((int)regionTOclient.Test, Test);
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
