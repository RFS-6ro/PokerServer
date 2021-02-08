﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Network;

namespace GameServer
{
	public class PokerClient : IClientBase
	{
		public int Id { get; set; }
		public TCPBase Tcp { get; set; }
		public UDPBase Udp { get; set; }
		public string UserName { get; set; }
		public string LobbyName { get; set; }

		public PokerClient(int id)
		{
			Id = id;

			Tcp = new TCP(Id);
			Udp = new UDP(Id);
		}

		void IClientBase.Disconnect()
		{
			Console.WriteLine($"{Tcp.Socket.Client.RemoteEndPoint} has disconnected.");

			Tcp.Disconnect();
			Udp.Disconnect();
		}

		public class TCP : TCPBase
		{
			private readonly int _id;

			public TCP(int id)
			{
				_id = id;
			}

			public override LoggerBase _logger => ConsoleLogger.Instance;

			public override void Connect(TcpClient socket = null)
			{
				Socket = socket;
				Socket.ReceiveBufferSize = NetworkSettings.DATA_BUFFER_SIZE;
				Socket.ReceiveBufferSize = NetworkSettings.DATA_BUFFER_SIZE;

				_stream = Socket.GetStream();

				_receivedPacket = new Packet();
				_receiveBuffer = new byte[NetworkSettings.DATA_BUFFER_SIZE];


				_stream.BeginRead(_receiveBuffer, 0, NetworkSettings.DATA_BUFFER_SIZE, ReceiveCallback, null);
				PokerSynchronisation.ServerPacketsSend.Welcome(_id, "Welcome to the master game server", ServerSendHandlers.SendTCPData);
			}

			public override void Disconnect()
			{
				_logger.PrintWarning(Thread.CurrentThread.ToString());
				Socket.Close();
				_stream = null;
				_receivedPacket = null;
				_receiveBuffer = null;
				Socket = null;
			}

			protected override void DisconnectClient()
			{
				Disconnect();
			}

			protected override void HandleData(int packetId, Packet packet)
			{
				_logger.PrintWarning(Thread.CurrentThread.ToString());
				IServer.PacketHandlers[packetId](_id, packet); // Call appropriate method to handle the packet
			}

			public void ReceiveCallbacks(IAsyncResult result)
			{
				_logger.PrintWarning(Thread.CurrentThread.ToString());
				_logger.PrintSuccess($"Receiving data {DateTime.Now}");
				try
				{
					int receivedLength = _stream.EndRead(result);
					_logger.PrintSuccess($"length = {receivedLength}");

					if (receivedLength <= 0)
					{
						DisconnectClient();

						_logger.PrintWarning("Zero bytes were received. Disconnecting...");

						return;
					}

					byte[] data = new byte[receivedLength];
					Array.Copy(_receiveBuffer, data, receivedLength);

					_logger.PrintSuccess($"start handling data at {DateTime.Now}");
					_receivedPacket.Reset(HandleReceivedData(data));

					_stream.BeginRead(_receiveBuffer, 0, NetworkSettings.DATA_BUFFER_SIZE, ReceiveCallback, null);
				}
				catch (Exception ex)
				{
					_logger.PrintError($"error with receiving data: {ex}");
					Disconnect();
				}
			}
		}

		public class UDP : UDPBase
		{
			private readonly int _id;

			public override LoggerBase _logger => ConsoleLogger.Instance;

			public UDP(int id)
			{
				_id = id;
			}

			public override void Connect(object connection)
			{
				if (connection.GetType() == typeof(IPEndPoint))
				{
					EndPoint = (IPEndPoint)connection;
				}
				else
				{
					Disconnect();
				}
			}

			public override void Disconnect()
			{
				EndPoint = null;
				Socket = null;
			}

			public override void HandleData(Packet data)
			{
				int packetLength = data.ReadInt();
				byte[] packetBytes = data.ReadBytes(packetLength);

				IThreadManager.ExecuteOnMainThread(() =>
				{
					using (Packet packet = new Packet(packetBytes))
					{
						int packetId = packet.ReadInt();
						IServer.PacketHandlers[packetId](_id, packet);
					}
				});
			}

			public override void SendData(Packet packet)
			{
				MainGameServer.Instance.SendUDPData(EndPoint, packet);
			}

			protected override void DisconnectClient()
			{
				Disconnect();
			}
		}
	}
}
