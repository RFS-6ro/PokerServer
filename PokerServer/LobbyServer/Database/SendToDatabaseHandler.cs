using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network.MessageHandlers;

namespace FrontendDistributionServer.Database
{
	public enum lobbyTOdatabase
	{
		None = 0,

		Count,


		Test
	}

	public class SendToDatabaseHandler : ISendMessageHandler<int>
	{
		public ISender Sender { get; set; }

		public Dictionary<int, Action<InitialSendingData>> Handlers { get; } = new Dictionary<int, Action<InitialSendingData>>();

		public SendToDatabaseHandler()
		{
			Handlers.Add((int)lobbyTOdatabase.Test, Test);
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
