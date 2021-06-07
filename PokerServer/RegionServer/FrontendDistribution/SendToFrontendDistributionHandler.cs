using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network.MessageHandlers;
using UniCastCommonData.Packet.InitialDatas;

namespace RegionServer.FrontendDistribution.Handlers
{
	public enum regionTOfrontend
	{
		None = 0,

		Connect,
		Disconnect,

		Count,


		Test
	}

	public class SendToFrontendDistributionHandler : ClientSender<Region_FrontendDistribution>
	{
		public SendToFrontendDistributionHandler()
		{
			Handlers.Add((int)regionTOfrontend.Test, Test);
			Handlers.Add((int)regionTOfrontend.Connect, Connect);
			Handlers.Add((int)regionTOfrontend.Disconnect, Disconnect);
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
