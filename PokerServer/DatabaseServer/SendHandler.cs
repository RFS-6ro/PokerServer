using System;
using DatabaseServer;
using UniCastCommonData;
using UniCastCommonData.Network.MessageHandlers;
using UniCastCommonData.Packet.InitialDatas;

namespace DatabaseServer
{
	public enum databaseTOany
	{
		None = 0,

		Connect,
		Disconnect,

		Count,


		Test
	}

	public class SendHandler : SessionSender<Database_Server>
	{
		public SendHandler()
		{
			Handlers.Add((int)databaseTOany.Test, Test);
			Handlers.Add((int)databaseTOany.Connect, Connect);
			Handlers.Add((int)databaseTOany.Disconnect, Disconnect);
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