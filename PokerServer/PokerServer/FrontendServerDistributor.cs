using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Network;
using static PokerSynchronisation.ClientPacketsSend;

namespace FrontendServer
{
	public class FrontendServerDistributor : AbstractServer, INeedLogger
	{
		protected static TcpListener _tcpListener;
		protected static UdpClient _udpListener;

		public LoggerBase _logger => ConsoleLogger.Instance;

		public override void Start(int maxPlayers, int port)
		{
			MaxPlayers = maxPlayers;
			Port = port;

			Console.WriteLine("Starting server...");
			InitializeServerData();

			_tcpListener = new TcpListener(IPAddress.Any, Port);
			_tcpListener.Start();
			_tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);

			_udpListener = new UdpClient(Port);
			_udpListener.BeginReceive(UDPReceiveCallback, null);

			Console.WriteLine($"Server started on port {Port}.");
		}

		protected override void InitializeServerData()
		{
			for (int i = 1; i <= MaxPlayers; i++)
			{
				Clients.Add(i, new FrontendClient(i));
			}

			PacketHandlers = new Dictionary<int, PacketHandler>()
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

			for (int i = 1; i <= MaxPlayers; i++)
			{
				if (Clients[i].Tcp.Socket == null)
				{
					Clients[i].Tcp.Connect(client);
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

					if (Clients[clientId].Udp.EndPoint == null)
					{
						// If this is a new connection
						Clients[clientId].Udp.Connect(clientEndPoint);
						return;
					}

					if (Clients[clientId].Udp.EndPoint.ToString() == clientEndPoint.ToString())
					{
						// Ensures that the client is not being impersonated by another by sending a false clientID
						Clients[clientId].Udp.HandleData(packet);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error receiving UDP data: {ex}");
			}
		}

		public static void SendUDPData(IPEndPoint clientEndPoint, Packet packet)
		{
			try
			{
				if (clientEndPoint != null)
				{
					_udpListener.BeginSend(packet.ToArray(), packet.Length, clientEndPoint, null, null);
				}
			}
			catch (Exception _ex)
			{
				Console.WriteLine($"Error sending data to {clientEndPoint} via UDP: {_ex}");
			}
		}
	}
}
