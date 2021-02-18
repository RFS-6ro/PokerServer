using System.Collections.Generic;
using Network;

namespace GameServer
{
	public static class MainGameServerSendToPlayersHandlers
	{
		/// <summary>Sends a packet to a client via TCP.</summary>
		/// <param name="toClient">The client to send the packet the packet to.</param>
		/// <param name="packet">The packet to send to the client.</param>
		public static void SendTCPData(int toClient, Packet packet)
		{
			packet.WriteLength();
			IServer.Clients[toClient].Tcp.SendData(packet);
		}

		/// <summary>Sends a packet to a client via TCP.</summary>
		/// <param name="toClient">The client to send the packet the packet to.</param>
		/// <param name="packet">The packet to send to the client.</param>
		public static void SendTCPData(IEnumerable<int> clients, Packet packet)
		{
			packet.WriteLength();
			foreach (var id in clients)
			{
				IServer.Clients[id].Tcp.SendData(packet);
			}
		}

		/// <summary>Sends a packet to a client via UDP.</summary>
		/// <param name="toClient">The client to send the packet the packet to.</param>
		/// <param name="packet">The packet to send to the client.</param>
		public static void SendUDPData(int toClient, Packet packet)
		{
			packet.WriteLength();
			IServer.Clients[toClient].Udp.SendData(packet);
		}

		/// <summary>Sends a packet to all clients via TCP.</summary>
		/// <param name="packet">The packet to send.</param>
		public static void SendTCPDataToAll(Packet packet)
		{
			packet.WriteLength();
			for (int i = 1; i <= IServer.MaxPlayers; i++)
			{
				IServer.Clients[i].Tcp.SendData(packet);
			}
		}
		/// <summary>Sends a packet to all clients except one via TCP.</summary>
		/// <param name="exceptClient">The client to NOT send the data to.</param>
		/// <param name="packet">The packet to send.</param>
		public static void SendTCPDataToAll(int exceptClient, Packet packet)
		{
			packet.WriteLength();
			for (int i = 1; i <= IServer.MaxPlayers; i++)
			{
				if (i != exceptClient)
				{
					IServer.Clients[i].Tcp.SendData(packet);
				}
			}
		}

		/// <summary>Sends a packet to all clients via UDP.</summary>
		/// <param name="packet">The packet to send.</param>
		public static void SendUDPDataToAll(Packet packet)
		{
			packet.WriteLength();
			for (int i = 1; i <= IServer.MaxPlayers; i++)
			{
				IServer.Clients[i].Udp.SendData(packet);
			}
		}
		/// <summary>Sends a packet to all clients except one via UDP.</summary>
		/// <param name="exceptClient">The client to NOT send the data to.</param>
		/// <param name="packet">The packet to send.</param>
		public static void SendUDPDataToAll(int exceptClient, Packet packet)
		{
			packet.WriteLength();
			for (int i = 1; i <= IServer.MaxPlayers; i++)
			{
				if (i != exceptClient)
				{
					IServer.Clients[i].Udp.SendData(packet);
				}
			}
		}
	}
}
