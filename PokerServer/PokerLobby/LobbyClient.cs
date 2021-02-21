using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Network;
using PokerSynchronisation;
using TexasHoldem.Logic.Players;
using static PokerLobby.LobbyReceiveHandle;

namespace PokerLobby
{
	public class LobbyClient : SingletonBase<LobbyClient>
	{
		private string _ip { get; set; }
		private int _port { get; set; }
		public int Id = 0;
		public TCP Tcp;


		private static TexasHoldemGameLogic _game;

		public static Dictionary<int, RealPlayer> Players = new Dictionary<int, RealPlayer>();
		public static List<RealPlayerDecorator> Decorators = new List<RealPlayerDecorator>();


		public bool IsConnected { get; set; } = false;

		public string Name { get; private set; }
		public LoggerBase Logger => ConsoleLogger.Instance;

		private delegate void PacketHandler(Packet packet);
		private static Dictionary<int, PacketHandler> _packetHandlers;

		public void SetName(string name)
		{
			Name = name;
		}

		public bool TryConnectPlayer(int playerId, string name, out int serverSideIndex)
		{
			if (Players.Count < DefaultSyncValues.MaxPlayers)
			{
				//CHECK: register player in lobby
				for (int i = 0; i < 7; i++)
				{
					if (Players.ContainsKey(i) == false)
					{
						Players.Add(i, new RealPlayer(name, playerId));
						serverSideIndex = i;
						return true;
					}
				}
			}
			serverSideIndex = -1;
			return false;
		}

		public void SetReadyState(int playerId, bool state)
		{
			RealPlayer player = Players.FirstOrDefault((x) => x.Value.ServerId == playerId).Value;
			if (player != null)
			{
				player.IsReady = state;
			}
		}

		public async Task PerformGameLoop(int buyIn, int smallBlind)
		{
			AssignRealPlayersToInternalDecorators();

			_game = new TexasHoldemGameLogic(Decorators.Cast<IPlayer>().ToList(), buyIn, smallBlind);
			RealPlayerDecorator winner = (RealPlayerDecorator)await _game.Start();

			//CHECK: Disconnect winner player from lobby
			DisconnectPlayer(winner.ServerId);
			//TODO: Send event to player about disconnecting from this server
		}

		private static void AssignRealPlayersToInternalDecorators()
		{
			for (int i = 0; i < Players.Count; i++)
			{
				RealPlayerDecorator decorator = new RealPlayerDecorator();
#if !DEBUG
				decorator.DrawGameBox((6 * i) + 3, 66, 1);
#endif
				decorator.SetPlayer(Players[i]);
				Decorators.Add(decorator);
			}
		}

		public void DisconnectPlayer(int playerId)
		{
			try
			{
				Players.Remove(Players.Where((x) => x.Value.ServerId == playerId).ElementAt(0).Key);
			}
			catch (Exception ex)
			{
				//TODO: write in log that lobby is trying to disconnect missing player
				return;
			}
			//TODO: Send event to players and game loop about disconnecting from this server
		}

		/// <summary>Attempts to connect to the server.</summary>
		public void ConnectToServer(string ip = NetworkSettings.LOCAL_HOST_IP, int port = NetworkSettings.LOBBY_CONNECTION_PORT)
		{
			Disconnect();

			_ip = ip;
			_port = port;

			Logger.PrintWarning(_port.ToString());

			Tcp = new TCP();

			InitializeClientData();

			Tcp.Connect(); // Connect tcp, udp gets connected once tcp is done
			IsConnected = true;
		}

		private void InitializeClientData()
		{
			_packetHandlers = new Dictionary<int, PacketHandler>()
			{
				{ (int)LobbyReceiveTypes.LobbyConnectionWelcome,    LobbyConnectionWelcome  },
				{ (int)LobbyReceiveTypes.PlayerConnection,          PlayerConnect           },
				{ (int)LobbyReceiveTypes.PlayerReadyStateChanged,   PlayerReadyStateChanged },
				{ (int)LobbyReceiveTypes.PlayerDisconnection,       PlayerDisconnect        },
				{ (int)LobbyReceiveTypes.PlayersTurn,               PlayerTurn              },
			};
			Logger.PrintSuccess("Initialized packets.");
		}

		private void Disconnect()
		{
			if (IsConnected)
			{
				IsConnected = false;
				Tcp?.Socket?.Close();

				Logger.PrintWarning("Disconnected from server.");
			}
		}

		public class TCP
		{
			public TcpClient Socket;

			private NetworkStream _stream;
			private Packet _receivedPacket;
			private byte[] _receiveBuffer;

			private Action OnConnectCallback = null;

			/// <summary>Attempts to connect to the server via TCP.</summary>
			public void Connect(Action onConnectCallback = null)
			{
				Socket = new TcpClient
				{
					ReceiveBufferSize = NetworkSettings.DATA_BUFFER_SIZE,
					SendBufferSize = NetworkSettings.DATA_BUFFER_SIZE
				};

				_receiveBuffer = new byte[NetworkSettings.DATA_BUFFER_SIZE];

				OnConnectCallback = onConnectCallback;
				Socket.BeginConnect(_instance._ip, _instance._port, ConnectCallback, Socket);
			}

			/// <summary>Initializes the newly connected client's TCP-related info.</summary>
			private void ConnectCallback(IAsyncResult result)
			{
				Socket.EndConnect(result);

				if (!Socket.Connected)
				{
					return;
				}

				_stream = Socket.GetStream();

				_receivedPacket = new Packet();

				_stream.BeginRead(_receiveBuffer, 0, NetworkSettings.DATA_BUFFER_SIZE, ReceiveCallback, null);
				OnConnectCallback?.Invoke();
			}

			/// <summary>Sends data to the client via TCP.</summary>
			/// <param name="packet">The packet to send.</param>
			public void SendData(Packet packet)
			{
				try
				{
					if (Socket != null)
					{
						_stream.BeginWrite(packet.ToArray(), 0, packet.Length, null, null); // Send data to server
					}
				}
				catch (Exception ex)
				{
				}
			}

			/// <summary>Reads incoming data from the stream.</summary>
			private void ReceiveCallback(IAsyncResult result)
			{
				try
				{
					int byteLength = _stream.EndRead(result);
					if (byteLength <= 0)
					{
						Instance.Disconnect();
						return;
					}


					byte[] data = new byte[byteLength];
					Array.Copy(_receiveBuffer, data, byteLength);


					_receivedPacket.Reset(HandleData(data)); // Reset receivedData if all data was handled
					try
					{
						_stream.BeginRead(_receiveBuffer, 0, NetworkSettings.DATA_BUFFER_SIZE, ReceiveCallback, null);
					}
					catch (Exception ex)
					{
						Disconnect();
					}
				}
				catch (Exception ex)
				{
					Disconnect();
				}
			}

			/// <summary>Prepares received data to be used by the appropriate packet handler methods.</summary>
			/// <param name="data">The recieved data.</param>
			private bool HandleData(byte[] data)
			{
				int packetLength = 0;

				_receivedPacket.SetBytes(data);

				if (_receivedPacket.UnreadLength >= 4)
				{
					// If client's received data contains a packet
					byte[] allBytes = _receivedPacket.ReadBytes(_receivedPacket.UnreadLength, false);

					packetLength = _receivedPacket.ReadInt();
					if (packetLength <= 0)
					{
						// If packet contains no data
						return true; // Reset _receivedPacket instance to allow it to be reused
					}
				}

				while (packetLength > 0 && packetLength <= _receivedPacket.UnreadLength)
				{
					// While packet contains data AND packet data length doesn't exceed the length of the packet we're reading
					byte[] packetBytes = _receivedPacket.ReadBytes(packetLength);
					//IThreadManager.ExecuteOnMainThread(() =>
					//{
					using (Packet packet = new Packet(packetBytes))
					{
						int packetId = packet.ReadInt();
						_packetHandlers[packetId](packet);
					}
					//});

					packetLength = 0; // Reset packet length
					if (_receivedPacket.UnreadLength >= 4)
					{
						// If client's received data contains another packet
						packetLength = _receivedPacket.ReadInt();
						if (packetLength <= 0)
						{
							// If packet contains no data
							return true; // Reset _receivedPacket instance to allow it to be reused
						}
					}
				}

				if (packetLength <= 1)
				{
					return true; // Reset _receivedPacket instance to allow it to be reused
				}

				return false;
			}

			/// <summary>Disconnects from the server and cleans up the TCP connection.</summary>
			private void Disconnect()
			{
				_instance.Disconnect();

				_stream = null;
				_receivedPacket = null;
				_receiveBuffer = null;
				Socket = null;
			}
		}

	}
}
