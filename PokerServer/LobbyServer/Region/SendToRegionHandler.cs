using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network.MessageHandlers;
using UniCastCommonData.Packet.InitialDatas;

namespace LobbyServer.Region.Handlers
{
	public enum lobbyTOregion
	{
		None = 0,

		Connect,
		Disconnect,

		Count,


		Test
	}

	public class SendToRegionHandler : ClientSender<Lobby_Region>
	{
		public SendToRegionHandler()
		{
			Handlers.Add((int)lobbyTOregion.Test, Test);
			Handlers.Add((int)lobbyTOregion.Connect, Connect);
			Handlers.Add((int)lobbyTOregion.Disconnect, Disconnect);
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