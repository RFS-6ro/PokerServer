using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace PokerServer
{
	public static class Server
	{
		private static TcpListener _tcpListener;

		public static Dictionary<int, Client> Clients = new Dictionary<int, Client>();

		public static int MaxPlayers { get; private set; }
		public static int Port { get; private set; }

		public static void Start(int maxPlayers, int port)
		{
			MaxPlayers = maxPlayers;
			Port = port;

			Console.WriteLine("Starting poker server...");
			Console.WriteLine("Initialising server data...");
			InitializeServerData();

			Console.WriteLine("Creating TCP receiver...");
			_tcpListener = new TcpListener(IPAddress.Any, Port);
			_tcpListener.Start();
			_tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);
			ConsoleHandler.PrintSuccess($"Server has been started at port {Port}.");
		}

		private static void TCPConnectCallback(IAsyncResult result)
		{
			TcpClient client = _tcpListener.EndAcceptTcpClient(result);
			_tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

			Console.WriteLine($"Incoming connection from {client.Client.RemoteEndPoint}...");

			for (int i = 1; i <= MaxPlayers; i++)
			{
				if (Clients[i].Tcp.Socket == null)
				{
					Clients[i].Tcp.Connect(client);

					if (i == MaxPlayers)
					{
						ConsoleHandler.PrintWarning("Server is full");
					}

					return;
				}
			}

			ConsoleHandler.PrintError($"{client.Client.RemoteEndPoint} failed to connect. Server is full");
		}

		private static void InitializeServerData()
		{
			for (int i = 1; i <= MaxPlayers; i++)
			{
				Clients.Add(i, new Client(i));
			}
		}
	}
}
