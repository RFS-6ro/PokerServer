using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Network;
using PokerSynchronisation;
using static PokerSynchronisation.ClientPacketsSend;
using static PokerSynchronisation.LobbySends;

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

			//LobbyPoolhandler.Instance.CreateNewLobby(new LobbyIdentifierData("LOBBY_2", 6, 3, 600));
			//LobbyPoolhandler.Instance.CreateNewLobby(new LobbyIdentifierData("LOBBY_3", 7, 4, 700));
			//LobbyPoolhandler.Instance.CreateNewLobby(new LobbyIdentifierData("LOBBY_4", 5, 2, 500));
			//LobbyPoolhandler.Instance.CreateNewLobby(new LobbyIdentifierData("LOBBY_5", 6, 3, 600));
			//LobbyPoolhandler.Instance.CreateNewLobby(new LobbyIdentifierData("LOBBY_6", 7, 4, 700));
			//LobbyPoolhandler.Instance.CreateNewLobby(new LobbyIdentifierData("LOBBY_7", 5, 2, 500));
			//LobbyPoolhandler.Instance.CreateNewLobby(new LobbyIdentifierData("LOBBY_8", 6, 3, 600));
			//LobbyPoolhandler.Instance.CreateNewLobby(new LobbyIdentifierData("LOBBY_9", 7, 4, 700));
			//LobbyPoolhandler.Instance.CreateNewLobby(new LobbyIdentifierData("LOBBY_10", 5, 5, 500));
		}

		public void ConnectToLobby(int id, string lobbyName)
		{
			bool result;

			try
			{
				LobbyProcessData lobbyProcessData = LobbyPoolhandler.Instance.GetLobbyByName(lobbyName);

				result = false;

				//TODO: Check connect player by id to lobby

				if (result)
				{
					((PokerClient)IServer.Clients[id]).LobbyName = lobbyProcessData.LobbyIdentifierData.Name;

					//Send message about success/error of connection lobby
					ServerPacketsSend.ConnectionToLobbyApprovance(id, lobbyProcessData.LobbyIdentifierData, MainGameServerSendHandlers.SendTCPData);
					//Send connect message for everyone except id
					ServerPacketsSend.SendPlayerActionToLobbyPlayers(id, ((PokerClient)IServer.Clients[id]).UserName, 0, true, MainGameServerSendHandlers.SendTCPDataToAll);
					List<LobbySeatData> seatDatas = new List<LobbySeatData>();
					//TODO: Fill lobby seats data
					//Send all lobby info to connected player
					ServerPacketsSend.SendLobbyData(id, seatDatas, MainGameServerSendHandlers.SendTCPData);
				}
				else
				{
					//Send message about success/error of connection lobby
					ServerPacketsSend.ConnectionToLobbyApprovance(id, "You couldn't connect to this lobby", MainGameServerSendHandlers.SendTCPData);
				}
			}
			catch (Exception ex)
			{
				result = false;
				_logger.PrintError($"Player by ID: {id} invoked error with connecting to lobby.");
				_logger.PrintError(ex.ToString());
				//Send message about success/error of connection lobby
				ServerPacketsSend.ConnectionToLobbyApprovance(id, "Some errors were occured with connecting", MainGameServerSendHandlers.SendTCPData);
			}

			ConsoleLogger.Instance.PrintColored(result.ToString(), ConsoleColor.Blue, ConsoleColor.DarkYellow);
		}

		public void ExitLobby(int id, string lobbyName)
		{
			try
			{
				LobbyProcessData lobbyData = LobbyPoolhandler.Instance.GetLobbyByName(lobbyName);
				//Send disconnect message for everyone except id
				ServerPacketsSend.SendPlayerActionToLobbyPlayers(id, ((PokerClient)IServer.Clients[id]).UserName, 0, false, MainGameServerSendHandlers.SendTCPDataToAll);
			}
			catch { }

			((PokerClient)IServer.Clients[id]).LobbyName = null;
			//TODO: Disconnect player by id from lobby
			//GameServerSends.PlayerDisconnect()
		}

		public void InitializeServerData()
		{
			for (int i = 1; i <= IServer.MaxPlayers; i++)
			{
				IServer.Clients.Add(i, new PokerClient(i));
			}

			for (int i = 1; i <= IServer.MaxPlayers / 3; i++)
			{
				LobbyNetworkBunch bunch = new LobbyNetworkBunch(i, new LobbyClient(i));
				bunch.ResetLobby();

				Lobbies.Add(i, bunch);
			}

			IServer.PacketHandlers = new Dictionary<int, IServer.PacketHandler>()
			{
				#region Client
				{ (int)ClientPacketsToServer.WelcomeReceived,   MainGameServerReceivedFromPlayerHandle.WelcomeReceived },
				{ (int)ClientPacketsToServer.MakeTurn,          MainGameServerReceivedFromPlayerHandle.TurnReceive },
				{ (int)ClientPacketsToServer.ExitLobby,         MainGameServerReceivedFromPlayerHandle.ExitLobby },
				{ (int)ClientPacketsToServer.ConnectToLobby,    MainGameServerReceivedFromPlayerHandle.ChoseLobby },
				{ (int)ClientPacketsToServer.AskLobbiesList,    MainGameServerReceivedFromPlayerHandle.AskLobbyList },
				#endregion
				#region Lobby
				{ (int)LobbyPacketsToGameServer.WelcomeReceived,                MainGameServerReceivedFromLobbyHandle.WelcomeReceived              },
				{ (int)LobbyPacketsToGameServer.TimerEvent,                     MainGameServerReceivedFromLobbyHandle.TimerEvent                   },
				{ (int)LobbyPacketsToGameServer.TurnApprovance,                 MainGameServerReceivedFromLobbyHandle.TurnApprovance               },
				{ (int)LobbyPacketsToGameServer.StartTurn,                      MainGameServerReceivedFromLobbyHandle.StartTurn                    },
				{ (int)LobbyPacketsToGameServer.ShowBank,                       MainGameServerReceivedFromLobbyHandle.ShowBank                     },
				{ (int)LobbyPacketsToGameServer.ConnectionToLobbyApprovance,    MainGameServerReceivedFromLobbyHandle.ConnectionToLobbyApprovance  },
				{ (int)LobbyPacketsToGameServer.ShowMoneyLeft,                  MainGameServerReceivedFromLobbyHandle.ShowPlayerMoney              },
				{ (int)LobbyPacketsToGameServer.Dealer,                         MainGameServerReceivedFromLobbyHandle.Dealer                       },
				{ (int)LobbyPacketsToGameServer.GiveCard,                       MainGameServerReceivedFromLobbyHandle.GiveCard                     },
				{ (int)LobbyPacketsToGameServer.ShowTableCard,                  MainGameServerReceivedFromLobbyHandle.ShowTableCard                },
				{ (int)LobbyPacketsToGameServer.WinAmount,                      MainGameServerReceivedFromLobbyHandle.WinAmount                    },
				{ (int)LobbyPacketsToGameServer.ShowPlayerBet,                  MainGameServerReceivedFromLobbyHandle.ShowPlayerBet                },
				{ (int)LobbyPacketsToGameServer.EndTurn,                        MainGameServerReceivedFromLobbyHandle.EndTurn                      },
				{ (int)LobbyPacketsToGameServer.CollectAllBets,                 MainGameServerReceivedFromLobbyHandle.CollectAllBets               },
				{ (int)LobbyPacketsToGameServer.ShowAllCards,                   MainGameServerReceivedFromLobbyHandle.ShowAllCards                 },
				#endregion
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
