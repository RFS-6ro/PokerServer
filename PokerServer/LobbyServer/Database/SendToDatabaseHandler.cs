using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network.MessageHandlers;
using UniCastCommonData.Packet.InitialDatas;

namespace FrontendDistributionServer.Database
{
	public enum lobbyTOdatabase
	{
		None = 0,

		Connect,
		Disconnect,

		Count,


		Test
	}

	public class SendToDatabaseHandler : ClientSender<Lobby_Database>
	{
		public SendToDatabaseHandler()
		{
			Handlers.Add((int)lobbyTOdatabase.Test, Test);
			Handlers.Add((int)lobbyTOdatabase.Connect, Connect);
			Handlers.Add((int)lobbyTOdatabase.Disconnect, Disconnect);
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
