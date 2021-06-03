using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network.MessageHandlers;

namespace LobbyServer.Client.Handlers
{
	public enum lobbyTOclient
	{
		None = 0,

		Count,


		Test
	}

	public class SendToClientHandler : SessionSender<Lobby_Client_Server>
	{
		public SendToClientHandler()
		{
			Handlers.Add((int)lobbyTOclient.Test, Test);
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
