using System;
using UniCastCommonData;
using UniCastCommonData.Network.MessageHandlers;
using UniCastCommonData.Packet.InitialDatas;

namespace LobbyServer.Client.Handlers
{
	public enum lobbyTOclient
	{
		None = 0,

		Connect,
		Disconnect,

		Count,


		Test
	}

	public class SendToClientHandler : SessionSender<Lobby_Client_Server>
	{
		public SendToClientHandler()
		{
			Handlers.Add((int)lobbyTOclient.Test, Test);
			Handlers.Add((int)lobbyTOclient.Connect, Connect);
			Handlers.Add((int)lobbyTOclient.Disconnect, Disconnect);
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
