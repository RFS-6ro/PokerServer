using System;
using System.Collections.Generic;
using Network;

namespace GameServer
{
	public static class MainGameServerSendToLobbiesHandlers
	{
		/// <summary>Sends a packet to a client via TCP.</summary>
		/// <param name="toClient">The client to send the packet the packet to.</param>
		/// <param name="packet">The packet to send to the client.</param>
		public static void SendTCPData(int toClient, Packet packet)
		{
			packet.WriteLength();
			MainGameServer.Lobbies[toClient].Client?.Tcp.SendData(packet);
		}

		/// <summary>Sends a packet to a client via TCP.</summary>
		/// <param name="toClient">The client to send the packet the packet to.</param>
		/// <param name="packet">The packet to send to the client.</param>
		public static void SendTCPData(IEnumerable<int> clients, Packet packet)
		{
			packet.WriteLength();
			foreach (var id in clients)
			{
				MainGameServer.Lobbies[id].Client.Tcp.SendData(packet);
			}
		}

		/// <summary>Sends a packet to all clients via TCP.</summary>
		/// <param name="packet">The packet to send.</param>
		public static void SendTCPDataToAll(Packet packet)
		{
			packet.WriteLength();
			for (int i = 1; i <= MainGameServer.Lobbies.Count; i++)
			{
				MainGameServer.Lobbies[i].Client.Tcp.SendData(packet);
			}
		}
		/// <summary>Sends a packet to all clients except one via TCP.</summary>
		/// <param name="exceptClient">The client to NOT send the data to.</param>
		/// <param name="packet">The packet to send.</param>
		public static void SendTCPDataToAll(int exceptClient, Packet packet)
		{
			packet.WriteLength();
			for (int i = 1; i <= MainGameServer.Lobbies.Count; i++)
			{
				if (i != exceptClient)
				{
					MainGameServer.Lobbies[i].Client.Tcp.SendData(packet);
				}
			}
		}
	}
}
