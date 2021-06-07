using System;
using UniCastCommonData;
using UniCastCommonData.Network.MessageHandlers;
using UniCastCommonData.Packet.InitialDatas;

namespace RegionServer.Lobby.Handlers
{
	public enum regionTOlobby
	{
		None = 0,

		Connect,
		Disconnect,

		Count,


		Test
	}

	public class SendToLobbyHandler : SessionSender<Region_Lobby_Server>
	{
		public SendToLobbyHandler()
		{
			Handlers.Add((int)regionTOlobby.Test, Test);
			Handlers.Add((int)regionTOlobby.Connect, Connect);
			Handlers.Add((int)regionTOlobby.Disconnect, Disconnect);
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