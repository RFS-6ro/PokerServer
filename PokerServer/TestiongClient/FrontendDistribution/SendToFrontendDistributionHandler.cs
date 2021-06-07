using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network.MessageHandlers;
using UniCastCommonData.Packet.InitialDatas;

namespace TestingClient.FrontendDistribution.Handlers
{
	public enum clientTOfrontend
	{
		None = 0,

		Connect,
		Disconnect,

		Count,


		Test
	}

	public class SendToFrontendDistributionHandler : ClientSender<Client_FrontendDistributor>
	{
		public SendToFrontendDistributionHandler()
		{
			Handlers.Add((int)clientTOfrontend.Test, Test);
			Handlers.Add((int)clientTOfrontend.Connect, Connect);
			Handlers.Add((int)clientTOfrontend.Disconnect, Disconnect);
		}

		private void Disconnect(InitialSendingData data)
		{
			SendAsync(data, new byte[][]
			{
			});
		}

		private void Connect(InitialSendingData data)
		{
			SendAsync(data, new byte[][]
			{
			});
		}

		private void Test(InitialSendingData data)
		{
			SendAsync(data, new byte[][]
			{
				GetType().ToString().ToByteArray()
			});
		}
	}
}