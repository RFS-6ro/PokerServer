using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Network;
using PokerSynchronisation;
using TexasHoldem.Logic.Players;
using static PokerSynchronisation.GameServerSends;

namespace PokerLobby
{
	public class LobbyClient : SingletonBase<LobbyClient>
	{
		private string _ip { get; set; }
		private int _port { get; set; }
		public int Id = 0;
		public TCPBase Tcp;


		private static TexasHoldemGameLogic _game;

		public static List<RealPlayer> Players = new List<RealPlayer>();
		public static List<RealPlayerDecorator> Decorators = new List<RealPlayerDecorator>();


		private bool isConnected = false;

		public string Name { get; private set; }
		public LoggerBase Logger => ConsoleLogger.Instance;

		private delegate void PacketHandler(Packet packet);
		private static Dictionary<int, PacketHandler> _packetHandlers;

		public void SetName(string name)
		{
			Name = name;
		}

		public bool TryConnectPlayer(int playerId, string name)
		{
			if (Players.Count < DefaultSyncValues.MaxPlayers)
			{
				Players.Add(new RealPlayer(name, playerId));

				return true;
			}

			return false;
		}

		public void SetReadyState(int playerId, bool state)
		{
			RealPlayer player = Players.FirstOrDefault((x) => x.ServerId == playerId);
			if (player != null)
			{
				player.IsReady = state;
			}
		}

		public async Task PerformGameLoop()
		{
			AssignRealPlayersToInternalDecorators();

			_game = new TexasHoldemGameLogic(Decorators.Cast<IPlayer>().ToList());
			RealPlayerDecorator winner = (RealPlayerDecorator)await _game.Start();

			//CHECK: Disconnect winner player from lobby
			Players.RemoveAll((x) => x.ServerId == winner.ServerId);
			//TODO: Send event to players about disconnecting from this server
		}

		private static void AssignRealPlayersToInternalDecorators()
		{
			for (int i = 0; i < Players.Count; i++)
			{
				RealPlayerDecorator decorator = new RealPlayerDecorator();
#if DEBUG
				decorator.DrawGameBox((6 * i) + 3, 66, 1);
#endif
				decorator.SetPlayer(Players[i]);
				Decorators.Add(decorator);
			}
		}

		public void DisconnectPlayer(int playerId)
		{
			Players.RemoveAll((x) => x.ServerId == playerId);
			//TODO: Send event to players about disconnecting from this server
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
				{ (int)GameServerToLobbyPackets.Welcome, ClientReceiveHandle.Welcome },
				{ (int)GameServerToLobbyPackets.PlayerConnect, ClientReceiveHandle.PlayerConnect },
				{ (int)GameServerToLobbyPackets.PlayerDisconnect, ClientReceiveHandle.PlayerDisconnect },
				{ (int)GameServerToLobbyPackets.PlayerTurn, ClientReceiveHandle.PlayerTurn },
				{ (int)GameServerToLobbyPackets.PlayerReadyStateChanged, ClientReceiveHandle.PlayerReadyStateChanged },
			};
			Logger.PrintSuccess("Initialized packets.");
		}

		private void Disconnect()
		{
			if (isConnected)
			{
				isConnected = false;
				Tcp?.Socket?.Close();

				Logger.PrintWarning("Disconnected from server.");
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
