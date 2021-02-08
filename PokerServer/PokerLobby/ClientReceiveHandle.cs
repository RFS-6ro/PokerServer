using System;
using Network;

namespace PokerLobby
{
	public class ClientReceiveHandle
	{
		public static void Welcome(Packet packet)
		{
			int id = packet.ReadInt();
			string message = packet.ReadString();

			ConsoleLogger.Instance.Print($"Message from server: {message}");
			LobbyClient.Instance.Id = id;
			ClientSendHandle.WelcomeReceived();
		}
	}
}