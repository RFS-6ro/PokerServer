using System;
using System.Collections.Generic;
using System.Linq;
using Network;
using PokerSynchronisation;

namespace GameServer
{
	public static class MainGameServerReceivedFromPlayerHandle
	{
		public enum MainGameServerReceivedFromPlayerTypes
		{
			WelcomeReceived = 101,

			PlayerConnection = 102,
			PlayerReadyStateChanged = 103,
			PlayerDisconnection = 104,

			PlayersTurn = 105,
			AskLobbyList = 106,
		}

		public static void WelcomeReceived(int fromClient, Packet packet)
		{
			ConsoleLogger.Instance.Print("FUCK");
			int clientIdCheck = packet.ReadInt();
			string userName = packet.ReadString();

			((PokerClient)IServer.Clients[clientIdCheck]).UserName = userName;

			Console.WriteLine($"{ IServer.Clients[fromClient].Tcp.Socket.Client.RemoteEndPoint } connected successfully and is now player { fromClient } with name { userName }.");
			if (fromClient != clientIdCheck)
			{
				Console.WriteLine($"Player \"{ userName }\" (ID: { fromClient }) has assumed the wrong client ID ({ clientIdCheck })!");
			}
		}

		public static void PlayerConnection(int fromClient, Packet packet)
		{
			int clientIdCheck = packet.ReadInt();
			string playerName = packet.ReadString();
			int lobbyId = packet.ReadInt();

			if (fromClient != clientIdCheck)
			{
				Console.WriteLine($"Player (ID: { fromClient }) has assumed the wrong client ID ({ clientIdCheck })!");
			}

			MainGameServer.Instance.ConnectToLobby(clientIdCheck, playerName, lobbyId);
		}

		public static void PlayerDisconnection(int fromClient, Packet packet)
		{
			int clientIdCheck = packet.ReadInt();
			//CHECK: Get lobby id by player Id
			int lobbyId = -1;
			try
			{
				lobbyId = MainGameServer.Lobbies.FirstOrDefault((x) => x.Value.IsAssigned && x.Value.Client.RegisteredPlayers.Contains(clientIdCheck)).Key;

				if (fromClient != clientIdCheck)
				{
					Console.WriteLine($"Player (ID: { fromClient }) has assumed the wrong client ID ({ clientIdCheck })!");
				}

				MainGameServer.Instance.ExitLobby(clientIdCheck, lobbyId);
			}
			catch (Exception ex)
			{

			}
		}

		public static void PlayerReadyStateChanged(int fromClient, Packet packet)
		{
			int clientIdCheck = packet.ReadInt();
			bool isReady = packet.ReadBool();
			//CHECK: Get lobby id by player Id
			int lobbyId = -1;
			try
			{
				lobbyId = MainGameServer.Lobbies.FirstOrDefault((x) => x.Value.IsAssigned && x.Value.Client.RegisteredPlayers.Contains(clientIdCheck)).Key;

				if (fromClient != clientIdCheck)
				{
					Console.WriteLine($"Player (ID: { fromClient }) has assumed the wrong client ID ({ clientIdCheck })!");
				}

				MainGameServerSendsToLobbyHandle.PlayerReadyStateChanged(lobbyId, clientIdCheck, isReady);
			}
			catch (Exception ex)
			{

			}
		}

		public static void TurnReceive(int fromClient, Packet packet)
		{
			int playerId = packet.ReadInt();
			TurnType turnType = (TurnType)packet.ReadInt();
			int amount = packet.ReadInt();

			if (fromClient != playerId)
			{
				Console.WriteLine($"Player (ID: { fromClient }) has assumed the wrong client ID ({ playerId })!");
				MainGameServerSendsToPlayerHandle.TurnApprovance(playerId, false);
				return;
			}

			//CHECK: Get lobby id by player Id
			int lobbyId = -1;
			try
			{
				lobbyId = MainGameServer.Lobbies.FirstOrDefault((x) => x.Value.IsAssigned && x.Value.Client.RegisteredPlayers.Contains(playerId)).Key;

				if (fromClient != playerId)
				{
					Console.WriteLine($"Player (ID: { fromClient }) has assumed the wrong client ID ({ playerId })!");
				}

				//CHECK: Send turn to lobby and check turn avaliability
				MainGameServerSendsToLobbyHandle.PlayerTurn(lobbyId, playerId, turnType, amount);
			}
			catch (Exception ex)
			{
				MainGameServerSendsToPlayerHandle.TurnApprovance(playerId, false);
			}
		}

		public static void AskLobbyList(int fromClient, Packet packet)
		{
			int clientIdCheck = packet.ReadInt();

			if (fromClient != clientIdCheck)
			{
				Console.WriteLine($"Player (ID: { fromClient }) has assumed the wrong client ID ({ clientIdCheck })!");
			}

			List<LobbyIdentifierData> lobbies = MainGameServer.Lobbies
			.Where((x) => x.Value.IsAssigned)
			.Select((x) =>
			{
				LobbyIdentifierData data = new LobbyIdentifierData();
				LobbyNetworkBunch lobby = x.Value;

				data.ID = lobby.ID;
				data.Name = lobby.Name;
				data.BuyIn = lobby.BuyIn;
				data.SmallBlind = lobby.SmallBlind;
				data.NumberOfPlayers = lobby.Client.RegisteredPlayers.Count;

				return data;
			}).ToList();
			//CHECK: Send data
			MainGameServerSendsToPlayerHandle.SendLobbyList(clientIdCheck, lobbies);
		}
	}
}
