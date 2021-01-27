﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Network;
using static PokerSynchronisation.ClientPacketsSend;

namespace GameServer
{
	public class MainGameServer : SingletonBase<MainGameServer>, IServer, INeedLogger
	{
		protected static TcpListener _tcpListener;
		protected static UdpClient _udpListener;

		public LoggerBase _logger => ConsoleLogger.Instance;

		public void Start(int maxPlayers, int port)
		{
			IServer.MaxPlayers = maxPlayers;
			IServer.Port = port;

			Console.WriteLine("Starting server...");
			InitializeServerData();

			_tcpListener = new TcpListener(IPAddress.Any, IServer.Port);
			_tcpListener.Start();
			_tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);

			_udpListener = new UdpClient(IServer.Port);
			_udpListener.BeginReceive(UDPReceiveCallback, null);

			Console.WriteLine($"Server started on port {IServer.Port}.");
		}

		public void InitializeServerData()
		{
			for (int i = 1; i <= IServer.MaxPlayers; i++)
			{
				IServer.Clients.Add(i, new PokerClient(i));
			}

			IServer.PacketHandlers = new Dictionary<int, IServer.PacketHandler>()
			{
				{ (int)ClientPacketsToServer.WelcomeReceived, ServerHandle.WelcomeReceived },
			};
			Console.WriteLine("Initialized packets.");
		}

		void TCPConnectCallback(IAsyncResult result)
		{
			TcpClient client = _tcpListener.EndAcceptTcpClient(result);
			_tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);
			Console.WriteLine($"Incoming connection from {client.Client.RemoteEndPoint}...");

			for (int i = 1; i <= IServer.MaxPlayers; i++)
			{
				if (IServer.Clients[i].Tcp.Socket == null)
				{
					IServer.Clients[i].Tcp.Connect(client);
					return;
				}
			}

			Console.WriteLine($"{client.Client.RemoteEndPoint} failed to connect: Server full!");
		}

		void UDPReceiveCallback(IAsyncResult result)
		{
			try
			{
				IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
				byte[] data = _udpListener.EndReceive(result, ref clientEndPoint);
				_udpListener.BeginReceive(UDPReceiveCallback, null);

				if (data.Length < 4)
				{
					return;
				}

				using (Packet packet = new Packet(data))
				{
					int clientId = packet.ReadInt();

					if (clientId == 0)
					{
						return;
					}

					if (IServer.Clients[clientId].Udp.EndPoint == null)
					{
						// If this is a new connection
						IServer.Clients[clientId].Udp.Connect(clientEndPoint);
						return;
					}

					if (IServer.Clients[clientId].Udp.EndPoint.ToString() == clientEndPoint.ToString())
					{
						// Ensures that the client is not being impersonated by another by sending a false clientID
						IServer.Clients[clientId].Udp.HandleData(packet);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error receiving UDP data: {ex}");
			}
		}

		public void SendUDPData(IPEndPoint clientEndPoint, Packet packet)
		{
			try
			{
				if (clientEndPoint != null)
				{
					_udpListener.BeginSend(packet.ToArray(), packet.Length, clientEndPoint, null, null);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error sending data to {clientEndPoint} via UDP: {ex}");
			}
		}

		public void LaunchLobbyWithArgs(string[] args)
		{
			Process.Start("/Users/RFS_6ro/Documents/GitHub/PokerServer/PokerServer/PokerLobby/bin/Release/net5.0/osx.10.12-x64/PokerLobby", args);
		}
	}
}
