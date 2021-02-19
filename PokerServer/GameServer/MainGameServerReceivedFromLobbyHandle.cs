using System;
using System.Linq;
using Network;
using PokerSynchronisation;
using TexasHoldem.Logic.Cards;

namespace GameServer
{
	public static class MainGameServerReceivedFromLobbyHandle
	{
		public enum MainGameServerReceivedFromLobbyTypes
		{
			WelcomeReceived = 1,
			ConnectionToLobbyApprovance = 2,

			#region Game Loop
			DealerPosition = 3,
			GiveCard = 4,
			ShowTableCards = 5,
			StartTurn = 6,
			TimerEvent = 7,
			TurnApprovance = 8,
			ShowPlayerBet = 9,
			ShowPlayerMoney = 10,
			EndTurn = 11,
			CollectAllBets = 12,
			ShowBank = 13,
			ShowAllCards = 14,
			WinAmount = 15,
			#endregion
		}

		public static void WelcomeReceived(int fromLobby, Packet packet)
		{
			ConsoleLogger.Instance.Print("fuck");
			int lobbyId = packet.ReadInt();
			string message = packet.ReadString();

			//CHECK: connect input client as lobby

			Console.WriteLine($"{ MainGameServer.Lobbies?[fromLobby]?.Client?.Tcp?.Socket?.Client?.RemoteEndPoint } connected successfully and is now lobby { fromLobby } with message { message }.");
			if (fromLobby != lobbyId)
			{
				Console.WriteLine($"Lobby \"{ message }\" (ID: { fromLobby }) has assumed the wrong ID ({ lobbyId })!");
			}
			MainGameServer.Lobbies[fromLobby].Client.IsConnected = true;
		}

		public static void ConnectionToLobbyApprovance(int fromLobby, Packet packet)
		{
			int lobbyId = packet.ReadInt();

			if (fromLobby != lobbyId)
			{
				Console.WriteLine($"Lobby (ID: { fromLobby }) has assumed the wrong ID ({ lobbyId })!");
			}

			int playerId = packet.ReadInt();
			bool result = packet.ReadBool();
			string message;

			if (result)
			{
				message = "Welcome to the lobby";
				MainGameServer.Lobbies[lobbyId].Client.AddConnection(playerId);
				//TODO: ask or receive here data about everyone in lobby ad send it to connected player only
			}
			else
			{
				message = packet.ReadString();
			}

			//CHECK: send message event to player
			MainGameServerSendsToPlayerHandle.ConnectionToLobbyApprovance(playerId, result, message);
		}

		public static void Dealer(int fromLobby, Packet packet)
		{
			int lobbyId = packet.ReadInt();

			if (fromLobby != lobbyId)
			{
				Console.WriteLine($"Lobby (ID: { fromLobby }) has assumed the wrong ID ({ lobbyId })!");
			}

			int playerId = packet.ReadInt();

			//CHECK: send event about dealing player to everyone
			MainGameServerSendsToPlayerHandle.DealerPosition(playerId);
		}

		public static void GiveCard(int fromLobby, Packet packet)
		{
			int lobbyId = packet.ReadInt();

			if (fromLobby != lobbyId)
			{
				Console.WriteLine($"Lobby (ID: { fromLobby }) has assumed the wrong ID ({ lobbyId })!");
			}

			int playerId = packet.ReadInt();
			int type = packet.ReadInt();
			int suit = packet.ReadInt();

			//CHECK: send concrete values directly to player
			MainGameServerSendsToPlayerHandle.GiveCard(playerId, type, suit, MainGameServerSendToPlayersHandlers.SendTCPData);
			//CHECK: send fake values to everyone except player
			MainGameServerSendsToPlayerHandle.GiveCard(playerId, (int)CardType.Count, (int)CardSuit.Shirt, MainGameServerSendToPlayersHandlers.SendTCPDataToAll);
		}

		public static void ShowTableCard(int fromLobby, Packet packet)
		{
			int lobbyId = packet.ReadInt();

			if (fromLobby != lobbyId)
			{
				Console.WriteLine($"Lobby (ID: { fromLobby }) has assumed the wrong ID ({ lobbyId })!");
			}

			int cardsNumber = packet.ReadInt();
			int[] types = new int[cardsNumber];
			int[] suits = new int[cardsNumber];
			int[] indexes = new int[cardsNumber];

			for (int i = 0; i < cardsNumber; i++)
			{
				types[i] = packet.ReadInt();
				suits[i] = packet.ReadInt();
				indexes[i] = packet.ReadInt();
			}

			//CHECK: Send card values to dealer model
			MainGameServerSendsToPlayerHandle.ShowTableCards(types, suits, indexes);
		}

		public static void StartTurn(int fromLobby, Packet packet)
		{
			int lobbyId = packet.ReadInt();

			if (fromLobby != lobbyId)
			{
				Console.WriteLine($"Lobby (ID: { fromLobby }) has assumed the wrong ID ({ lobbyId })!");
			}

			int playerId = packet.ReadInt();
			bool canRaise = packet.ReadBool();

			//CHECK: send turn event to player's view & everyone else
			MainGameServerSendsToPlayerHandle.StartTurn(playerId, canRaise);
		}

		public static void TimerEvent(int fromLobby, Packet packet)
		{
			int lobbyId = packet.ReadInt();

			if (fromLobby != lobbyId)
			{
				Console.WriteLine($"Lobby (ID: { fromLobby }) has assumed the wrong ID ({ lobbyId })!");
			}

			int playerId = packet.ReadInt();
			bool isDecreasing = packet.ReadBool();
			int timeLeft = packet.ReadInt();

			//CHECK: send event to all players
			MainGameServerSendsToPlayerHandle.TimerEvent(playerId, isDecreasing, timeLeft);
		}

		public static void TurnApprovance(int fromLobby, Packet packet)
		{
			int lobbyId = packet.ReadInt();

			if (fromLobby != lobbyId)
			{
				Console.WriteLine($"Lobby (ID: { fromLobby }) has assumed the wrong ID ({ lobbyId })!");
			}

			int playerId = packet.ReadInt();
			bool result = packet.ReadBool();

			//CHECK: Send event to this player
			MainGameServerSendsToPlayerHandle.TurnApprovance(playerId, result);
		}

		public static void ShowPlayerBet(int fromLobby, Packet packet)
		{
			int lobbyId = packet.ReadInt();

			if (fromLobby != lobbyId)
			{
				Console.WriteLine($"Lobby (ID: { fromLobby }) has assumed the wrong ID ({ lobbyId })!");
			}

			int playerId = packet.ReadInt();
			int betAmount = packet.ReadInt();
			int currentlyInPot = packet.ReadInt();

			//CHECK: send event about bet and it's amount to everyone
			MainGameServerSendsToPlayerHandle.ShowPlayerBet(playerId, betAmount, currentlyInPot);

		}

		public static void ShowPlayerMoney(int fromLobby, Packet packet)
		{
			int lobbyId = packet.ReadInt();

			if (fromLobby != lobbyId)
			{
				Console.WriteLine($"Lobby (ID: { fromLobby }) has assumed the wrong ID ({ lobbyId })!");
			}

			int numberOfPlayers = packet.ReadInt();
			int[] playersIds = new int[numberOfPlayers];
			int[] playersMoney = new int[numberOfPlayers];
			for (int i = 0; i < numberOfPlayers; i++)
			{
				playersIds[i] = packet.ReadInt();
				playersMoney[i] = packet.ReadInt();
			}

			//CHECK: send event about money balance of player to everyone
			MainGameServerSendsToPlayerHandle.ShowPlayersMoney(playersIds, playersMoney);
		}

		public static void EndTurn(int fromLobby, Packet packet)
		{
			int lobbyId = packet.ReadInt();

			if (fromLobby != lobbyId)
			{
				Console.WriteLine($"Lobby (ID: { fromLobby }) has assumed the wrong ID ({ lobbyId })!");
			}

			int playerId = packet.ReadInt();

			//CHECK: send event about en of turning to everyone
			MainGameServerSendsToPlayerHandle.EndTurn(playerId);
		}

		public static void CollectAllBets(int fromLobby, Packet packet)
		{
			int lobbyId = packet.ReadInt();

			if (fromLobby != lobbyId)
			{
				Console.WriteLine($"Lobby (ID: { fromLobby }) has assumed the wrong ID ({ lobbyId })!");
			}

			//CHECK: Send this empty event to all Players in lobbytimeLeft
			MainGameServerSendsToPlayerHandle.CollectAllBets();
		}

		public static void ShowBank(int fromLobby, Packet packet)
		{
			int lobbyId = packet.ReadInt();

			if (fromLobby != lobbyId)
			{
				Console.WriteLine($"Lobby (ID: { fromLobby }) has assumed the wrong ID ({ lobbyId })!");
			}

			int bankAmount = packet.ReadInt();

			//CHECK: Send event to everyone to update bank amount
			MainGameServerSendsToPlayerHandle.ShowBank(bankAmount);
		}

		public static void ShowAllCards(int fromLobby, Packet packet)
		{
			int lobbyId = packet.ReadInt();

			if (fromLobby != lobbyId)
			{
				Console.WriteLine($"Lobby (ID: { fromLobby }) has assumed the wrong ID ({ lobbyId })!");
			}

			int playersNumber = packet.ReadInt();

			int[] ids = new int[playersNumber];
			int[] firstTypes = new int[playersNumber];
			int[] firstSuits = new int[playersNumber];
			int[] secondTypes = new int[playersNumber];
			int[] secondSuits = new int[playersNumber];

			for (int i = 0; i < playersNumber; i++)
			{
				ids[i] = packet.ReadInt();
				firstTypes[i] = packet.ReadInt();
				firstSuits[i] = packet.ReadInt();
				secondTypes[i] = packet.ReadInt();
				secondSuits[i] = packet.ReadInt();
			}

			//CHECK: send event about real cards to everyone
			MainGameServerSendsToPlayerHandle.ShowAllCards(firstTypes, firstSuits, secondTypes, secondSuits, ids.ToList());
		}

		public static void WinAmount(int fromLobby, Packet packet)
		{
			int lobbyId = packet.ReadInt();

			if (fromLobby != lobbyId)
			{
				Console.WriteLine($"Lobby (ID: { fromLobby }) has assumed the wrong ID ({ lobbyId })!");
			}

			int playerId = packet.ReadInt();
			int amount = packet.ReadInt();

			//CHECK: send event about winning amount to everyone
			MainGameServerSendsToPlayerHandle.WinAmount(playerId, amount);
		}

		//public static void PlayerStateAtStartOfRound(int fromLobby, Packet packet)
		//{
		//	int lobbyId = packet.ReadInt();

		//	if (fromLobby != lobbyId)
		//	{
		//		Console.WriteLine($"Lobby (ID: { fromLobby }) has assumed the wrong ID ({ lobbyId })!");
		//	}

		//	int playerId = packet.ReadInt();
		//	int money = packet.ReadInt();
		//	int currentlyInPot = packet.ReadInt();

		//	//T0D0: Send check event to all lobby players
		//}
	}
}
