using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Network;
using PokerSynchronisation;

namespace GameServer
{
	public class LobbyClient
	{
		public int Id { get; set; }
		public TCPBase Tcp { get; set; }
		public string Name { get; set; }

		/// <summary>
		/// first is id
		/// second is server side index
		/// </summary>
		public List<Tuple<int, int>> RegisteredPlayers = new List<Tuple<int, int>>(DefaultSyncValues.MaxPlayers);

		public bool IsConnected = false;

		public LobbyClient(int id)
		{
			Id = id;

			Tcp = new TCP(Id);
		}

		public void AddConnection(int newPlayerId, int serverSideIndex)
		{
			RegisteredPlayers.Add(new Tuple<int, int>(newPlayerId, serverSideIndex));
		}

		public void RemoveConnection(int playerId)
		{
			RegisteredPlayers.RemoveAll((x) => x.Item1 == playerId);
		}

		public void Disconnect()
		{
			Console.WriteLine($"{Tcp.Socket.Client.RemoteEndPoint} has disconnected.");
			IsConnected = false;
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

				var a = MainGameServer.Instance;
				var b = IServer.Clients;
				_stream.BeginRead(_receiveBuffer, 0, NetworkSettings.DATA_BUFFER_SIZE, ReceiveCallback, null);
				MainGameServerSendsToLobbyHandle.Welcome(_id, "Welcome to the master game server");
			}

			public override void Disconnect()
			{
				Socket.Close();
				_stream = null;
				_receivedPacket = null;
				_receiveBuffer = null;
				Socket = null;
				MainGameServer.Lobbies[_id].Client.IsConnected = false;
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
