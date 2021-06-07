using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network.MessageHandlers;
using UniCastCommonData.Packet.InitialDatas;

namespace RegionServer.Client.Handlers
{
	public enum regionTOclient
	{
		None = 0,

		Connect,
		Disconnect,

		Count,


		Test
	}

	public class SendToClientHandler : SessionSender<Region_Client_Server>
	{
		public SendToClientHandler()
		{
			Handlers.Add((int)regionTOclient.Test, Test);
			Handlers.Add((int)regionTOclient.Connect, Connect);
			Handlers.Add((int)regionTOclient.Disconnect, Disconnect);
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
