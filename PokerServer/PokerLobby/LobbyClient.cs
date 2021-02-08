using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Network;
using PokerSynchronisation;

namespace PokerLobby
{
	public class LobbyClient : SingletonBase<LobbyClient>
	{
		private string _ip { get; set; }
		private int _port { get; set; }
		public int Id = 0;
		public TCPBase Tcp;

		public string Name { get; private set; }

		private bool isConnected = false;

		public LoggerBase _logger => ConsoleLogger.Instance;
		private delegate void PacketHandler(Packet packet);
		private static Dictionary<int, PacketHandler> _packetHandlers;

		public void SetName(string name)
		{
			Name = name;
		}

		/// <summary>Attempts to connect to the server.</summary>
		public void ConnectToServer(string ip = NetworkSettings.LOCAL_HOST_IP, int port = NetworkSettings.TEST_SERVERPORT)
		{
			Disconnect();

			_ip = ip;
			_port = port;

			Tcp = new TCP();

			InitializeClientData();

			isConnected = true;
			Tcp.Connect(); // Connect tcp, udp gets connected once tcp is done
		}

		private void InitializeClientData()
		{
			_packetHandlers = new Dictionary<int, PacketHandler>()
			{
				{ (int)ServerPacketsSend.ServerPacketsToClient.Welcome, ClientReceiveHandle.Welcome },
			};
			_logger.PrintSuccess("Initialized packets.");
		}

		private void Disconnect()
		{
			if (isConnected)
			{
				isConnected = false;
				Tcp?.Socket?.Close();

				_logger.PrintWarning("Disconnected from server.");
			}
		}

		private class TCP : TCPBase
		{
			public override LoggerBase _logger => ConsoleLogger.Instance;

			public override void Connect(TcpClient socket = null)
			{
				Socket = new TcpClient
				{
					ReceiveBufferSize = NetworkSettings.DATA_BUFFER_SIZE,
					SendBufferSize = NetworkSettings.DATA_BUFFER_SIZE
				};

				_receiveBuffer = new byte[NetworkSettings.DATA_BUFFER_SIZE];

				_logger.PrintWarning("begin connect");
				Socket.BeginConnect(_instance._ip, _instance._port, ConnectCallback, Socket);
			}

			private void ConnectCallback(IAsyncResult result)
			{
				_logger.PrintWarning("check connect");

				Socket.EndConnect(result);

				if (!Socket.Connected)
				{
					_logger.PrintError("no connection");
					return;
				}

				_logger.PrintWarning("begin read");
				_stream = Socket.GetStream();

				_receivedPacket = new Packet();

				_stream.BeginRead(_receiveBuffer, 0, NetworkSettings.DATA_BUFFER_SIZE, ReceiveCallback, null);

				_logger.PrintSuccess("Lobby did successfully connected to server and ready to use");
			}

			public override void Disconnect()
			{
				_logger.PrintError("disconnecting");
				DisconnectClient();

				_stream = null;
				_receivedPacket = null;
				_receiveBuffer = null;
				Socket = null;
			}

			protected override void DisconnectClient()
			{
				_instance.Disconnect();
			}

			protected override void HandleData(int packetId, Packet packet)
			{
				_packetHandlers[packetId](packet);
			}
		}
	}
}
