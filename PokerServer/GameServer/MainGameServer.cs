using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Network;
using PokerSynchronisation;
using static GameServer.MainGameServerReceivedFromLobbyHandle;
using static GameServer.MainGameServerReceivedFromPlayerHandle;

namespace GameServer
{
	public class MainGameServer : SingletonBase<MainGameServer>, IServer, INeedLogger
	{
		protected static TcpListener _tcpListener;
		protected static UdpClient _udpListener;

		public LoggerBase _logger => ConsoleLogger.Instance;

		public static Dictionary<int, LobbyNetworkBunch> Lobbies = new Dictionary<int, LobbyNetworkBunch>();

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

			LobbyIdentifierData lobbyIdentifier = new LobbyIdentifierData();
			//LobbyProcessData lobby = LobbyPoolhandler.Instance.CreateNewLobby(lobbyIdentifier);
			//Lobbies[1].AssignNewLobby("First test lobby", 3, 300, null);
		}

		public void ConnectToLobby(int playerId, string playerName, int lobbyId)
		{
			ConsoleLogger.Instance.PrintColored($"Player (\"{playerName}\", ID: {playerId}) is trying to connect to lobby (id: {lobbyId})", ConsoleColor.Blue, ConsoleColor.DarkYellow);
			if (Lobbies[lobbyId].Client.IsConnected)
			{
				//CHECK: Check connect player by id to lobby
				ConsoleLogger.Instance.Print("lobby is responding");
				MainGameServerSendsToLobbyHandle.PlayerConnect(lobbyId, playerId, playerName);
			}
			else
			{
				ConsoleLogger.Instance.PrintError("lobby is not responding");
				MainGameServerSendsToPlayerHandle.ConnectionToLobbyApprovance(playerId, false, "Lobby is not assigned");
			}
			{
				//bool result;

				//try
				//{
				//LobbyNetworkBunch lobbyProcessData = Lobbies[lobbyId];

				//result = false;

				//if (result)
				//{
				//	((PokerClient)IServer.Clients[playerId]).LobbyName = lobbyProcessData.LobbyIdentifierData.Name;

				//	//Send message about success/error of connection lobby
				//	ServerPacketsSend.ConnectionToLobbyApprovance(playerId, lobbyProcessData.LobbyIdentifierData, MainGameServerSendHandlers.SendTCPData);
				//	//Send connect message for everyone except id
				//	ServerPacketsSend.SendPlayerActionToLobbyPlayers(playerId, ((PokerClient)IServer.Clients[playerId]).UserName, 0, true, MainGameServerSendHandlers.SendTCPDataToAll);
				//	List<LobbySeatData> seatDatas = new List<LobbySeatData>();
				//	//T0D0: Fill lobby seats data
				//	//Send all lobby info to connected player
				//	ServerPacketsSend.SendLobbyData(playerId, seatDatas, MainGameServerSendHandlers.SendTCPData);
				//}
				//else
				//{
				//	//Send message about success/error of connection lobby
				//	ServerPacketsSend.ConnectionToLobbyApprovance(playerId, "You couldn't connect to this lobby", MainGameServerSendHandlers.SendTCPData);
				//}
				//}
				//catch (Exception ex)
				//{
				//	result = false;
				//	_logger.PrintError($"Player by ID: {playerId} invoked error with connecting to lobby.");
				//	_logger.PrintError(ex.ToString());
				//	//Send message about success/error of connection lobby
				//	ServerPacketsSend.ConnectionToLobbyApprovance(playerId, "Some errors were occured with connecting", MainGameServerSendHandlers.SendTCPData);
				//}
			}
		}

		public void ExitLobby(int playerId, int lobbyId)
		{
			//CHECK: Disconnect player by id from lobby
			MainGameServerSendsToLobbyHandle.PlayerDisconnect(lobbyId, playerId);
			Lobbies[lobbyId].Client.RemoveConnection(playerId);
			{
				//try
				//{
				//	LobbyProcessData lobbyData = LobbyPoolhandler.Instance.GetLobbyByName(lobbyName);
				//	//Send disconnect message for everyone except id
				//	ServerPacketsSend.SendPlayerActionToLobbyPlayers(playerId, ((PokerClient)IServer.Clients[playerId]).UserName, 0, false, MainGameServerSendHandlers.SendTCPDataToAll);
				//}
				//catch { }

				//((PokerClient)IServer.Clients[playerId]).LobbyName = null;
				//GameServerSends.PlayerDisconnect()
			}
		}

		public void InitializeServerData()
		{
			for (int i = 1; i <= IServer.MaxPlayers; i++)
			{
				IServer.Clients.Add(i, new PokerClient(i));
			}

			for (int i = 1; i <= 2; i++)//IServer.MaxPlayers / 3; i++)
			{
				LobbyNetworkBunch bunch = new LobbyNetworkBunch(i, new LobbyClient(i));

				Lobbies.Add(i, bunch);
			}

			IServer.PacketHandlers = new Dictionary<int, IServer.PacketHandler>()
			{
				{ (int)MainGameServerReceivedFromLobbyTypes.WelcomeReceived,             MainGameServerReceivedFromLobbyHandle.WelcomeReceived  },
				{ (int)MainGameServerReceivedFromLobbyTypes.ConnectionToLobbyApprovance, ConnectionToLobbyApprovance                            },
				{ (int)MainGameServerReceivedFromLobbyTypes.DealerPosition,              Dealer                                                 },
				{ (int)MainGameServerReceivedFromLobbyTypes.GiveCard,                    GiveCard                                               },
				{ (int)MainGameServerReceivedFromLobbyTypes.ShowTableCards,              ShowTableCard                                          },
				{ (int)MainGameServerReceivedFromLobbyTypes.StartTurn,                   StartTurn                                              },
				{ (int)MainGameServerReceivedFromLobbyTypes.TimerEvent,                  TimerEvent                                             },
				{ (int)MainGameServerReceivedFromLobbyTypes.TurnApprovance,              TurnApprovance                                         },
				{ (int)MainGameServerReceivedFromLobbyTypes.ShowPlayerBet,               ShowPlayerBet                                          },
				{ (int)MainGameServerReceivedFromLobbyTypes.ShowPlayerMoney,             ShowPlayerMoney                                        },
				{ (int)MainGameServerReceivedFromLobbyTypes.EndTurn,                     EndTurn                                                },
				{ (int)MainGameServerReceivedFromLobbyTypes.CollectAllBets,              CollectAllBets                                         },
				{ (int)MainGameServerReceivedFromLobbyTypes.ShowBank,                    ShowBank                                               },
				{ (int)MainGameServerReceivedFromLobbyTypes.ShowAllCards,                ShowAllCards                                           },
				{ (int)MainGameServerReceivedFromLobbyTypes.WinAmount,                   WinAmount                                              },
				{ (int)MainGameServerReceivedFromPlayerTypes.WelcomeReceived,            MainGameServerReceivedFromPlayerHandle.WelcomeReceived },
				{ (int)MainGameServerReceivedFromPlayerTypes.PlayerConnection,           PlayerConnection                                       },
				{ (int)MainGameServerReceivedFromPlayerTypes.PlayerReadyStateChanged,    PlayerReadyStateChanged                                },
				{ (int)MainGameServerReceivedFromPlayerTypes.PlayerDisconnection,        PlayerDisconnection                                    },
				{ (int)MainGameServerReceivedFromPlayerTypes.PlayersTurn,                TurnReceive                                            },
				{ (int)MainGameServerReceivedFromPlayerTypes.AskLobbyList,               AskLobbyList                                           },
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
	}
}
