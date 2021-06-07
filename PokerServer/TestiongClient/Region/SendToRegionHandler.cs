using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network.MessageHandlers;
using UniCastCommonData.Packet.InitialDatas;

namespace TestingClient.Region.Handlers
{
	public enum clientTOregion
	{
		None = 0,

		Connect,
		Disconnect,

		Count,


		Test
	}

	public class SendToRegionHandler : ClientSender<Client_Region>
	{
		public SendToRegionHandler()
		{
			Handlers.Add((int)clientTOregion.Test, Test);
			Handlers.Add((int)clientTOregion.Connect, Connect);
			Handlers.Add((int)clientTOregion.Disconnect, Disconnect);
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