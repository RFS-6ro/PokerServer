using System;
using Network;
using static PokerSynchronisation.ClientPacketsSend;

namespace PokerLobby
{
	public class ClientSendHandle
	{
		public static void WelcomeReceived()
		{
			using (Packet packet = new Packet((int)ClientPacketsToServer.WelcomeReceived))
			{
				packet.Write(LobbyClient.Instance.Id);
				packet.Write("LOBBY_NAME_" + LobbyClient.Instance.Name);

				ClientSentHandlers.SendTCPData(packet);
			}
		}
	}
}