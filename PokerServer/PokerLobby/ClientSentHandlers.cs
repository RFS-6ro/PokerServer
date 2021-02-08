using Network;

namespace PokerLobby
{
	public class ClientSentHandlers
	{
		/// <summary>Sends a packet to the server via TCP.</summary>
		/// <param name="packet">The packet to send to the sever.</param>
		public static void SendTCPData(Packet packet)
		{
			packet.WriteLength();
			LobbyClient.Instance.Tcp.SendData(packet);
		}
	}
}