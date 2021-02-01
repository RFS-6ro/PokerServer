﻿using Network;

namespace GameServer
{
	public static class ServerSendHandlers
	{
		/// <summary>Sends a packet to a client via TCP.</summary>
		/// <param name="_toClient">The client to send the packet the packet to.</param>
		/// <param name="_packet">The packet to send to the client.</param>
		public static void SendTCPData(int _toClient, Packet _packet)
		{
			_packet.WriteLength();
			IServer.Clients[_toClient].Tcp.SendData(_packet);
		}

		/// <summary>Sends a packet to a client via UDP.</summary>
		/// <param name="_toClient">The client to send the packet the packet to.</param>
		/// <param name="_packet">The packet to send to the client.</param>
		public static void SendUDPData(int _toClient, Packet _packet)
		{
			_packet.WriteLength();
			IServer.Clients[_toClient].Udp.SendData(_packet);
		}

		/// <summary>Sends a packet to all clients via TCP.</summary>
		/// <param name="_packet">The packet to send.</param>
		public static void SendTCPDataToAll(Packet _packet)
		{
			_packet.WriteLength();
			for (int i = 1; i <= IServer.MaxPlayers; i++)
			{
				IServer.Clients[i].Tcp.SendData(_packet);
			}
		}
		/// <summary>Sends a packet to all clients except one via TCP.</summary>
		/// <param name="_exceptClient">The client to NOT send the data to.</param>
		/// <param name="_packet">The packet to send.</param>
		public static void SendTCPDataToAll(int _exceptClient, Packet _packet)
		{
			_packet.WriteLength();
			for (int i = 1; i <= IServer.MaxPlayers; i++)
			{
				if (i != _exceptClient)
				{
					IServer.Clients[i].Tcp.SendData(_packet);
				}
			}
		}

		/// <summary>Sends a packet to all clients via UDP.</summary>
		/// <param name="_packet">The packet to send.</param>
		public static void SendUDPDataToAll(Packet _packet)
		{
			_packet.WriteLength();
			for (int i = 1; i <= IServer.MaxPlayers; i++)
			{
				IServer.Clients[i].Udp.SendData(_packet);
			}
		}
		/// <summary>Sends a packet to all clients except one via UDP.</summary>
		/// <param name="_exceptClient">The client to NOT send the data to.</param>
		/// <param name="_packet">The packet to send.</param>
		public static void SendUDPDataToAll(int _exceptClient, Packet _packet)
		{
			_packet.WriteLength();
			for (int i = 1; i <= IServer.MaxPlayers; i++)
			{
				if (i != _exceptClient)
				{
					IServer.Clients[i].Udp.SendData(_packet);
				}
			}
		}
	}
}
