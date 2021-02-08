using System;
using System.Net.Sockets;
using Network;

namespace GameServer
{
	public class LobbyClient
	{
		public int Id { get; set; }
		public TCPBase Tcp { get; set; }
		public string Name { get; set; }

		public LobbyClient(int id)
		{
			Id = id;

			Tcp = new TCP(Id);
		}

		public void Disconnect()
		{
			Console.WriteLine($"{Tcp.Socket.Client.RemoteEndPoint} has disconnected.");

			Tcp.Disconnect();
		}

		public class TCP : TCPBase
		{
			private readonly int _id;

			public override LoggerBase _logger => ConsoleLogger.Instance;

			public TCP(int id)
			{
				_id = id;
			}

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
				IServer.PacketHandlers[packetId](_id, packet); // Call appropriate method to handle the packet
			}
		}
	}
}
