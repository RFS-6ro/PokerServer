using System;
using Network;
using PokerSynchronisation;
using TexasHoldem.Logic.Cards;

namespace GameServer
{
	public static class LobbyPacketsHandler
	{
		public static void WelcomeReceived(int fromLobby, Packet packet)
		{
			int lobbyId = packet.ReadInt();
			string userName = packet.ReadString();

			//CHECK: connect input client as lobby
			MainGameServer.Lobbies[lobbyId].Name = userName;

			Console.WriteLine($"{ IServer.Clients[fromLobby].Tcp.Socket.Client.RemoteEndPoint } connected successfully and is now lobby { fromLobby } with name { userName }.");
			if (fromLobby != lobbyId)
			{
				Console.WriteLine($"Lobby \"{ userName }\" (ID: { fromLobby }) has assumed the wrong ID ({ lobbyId })!");
			}
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
			ServerPacketsSend.ShowTimer(playerId, isDecreasing, timeLeft, ServerSendHandlers.SendTCPDataToAll);
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
			ServerPacketsSend.Approvance(playerId, result, ServerSendHandlers.SendTCPData);
		}

		public static void PlayerStateAtStartOfRound(int fromLobby, Packet packet)
		{
			int lobbyId = packet.ReadInt();

			if (fromLobby != lobbyId)
			{
				Console.WriteLine($"Lobby (ID: { fromLobby }) has assumed the wrong ID ({ lobbyId })!");
			}

			int playerId = packet.ReadInt();
			int money = packet.ReadInt();
			int currentlyInPot = packet.ReadInt();

			//TODO: Send check event to all lobby players
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
			ServerPacketsSend.StartTurn(playerId, canRaise, ServerSendHandlers.SendTCPData);
			ServerPacketsSend.StartTurn(playerId, ServerSendHandlers.SendTCPDataToAll);
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
			ServerPacketsSend.ShowBank(bankAmount, ServerSendHandlers.SendTCPDataToAll);
		}

		public static void ConnectionToLobbyApprovance(int fromLobby, Packet packet)
		{
			int lobbyId = packet.ReadInt();

			if (fromLobby != lobbyId)
			{
				Console.WriteLine($"Lobby (ID: { fromLobby }) has assumed the wrong ID ({ lobbyId })!");
			}

			bool result = packet.ReadBool();
			int playerId = packet.ReadInt();
			string message;

			if (result)
			{
				message = "Welcome to the lobby";
				//TODO: send connection event to everyone except connected player
				//TODO: ask or receive here data about everyone in lobby ad send it to connectrd player only
			}
			else
			{
				message = packet.ReadString();
			}

			//TODO: send message event to player

		}

		public static void ShowMoneyLeft(int fromLobby, Packet packet)
		{
			int lobbyId = packet.ReadInt();

			if (fromLobby != lobbyId)
			{
				Console.WriteLine($"Lobby (ID: { fromLobby }) has assumed the wrong ID ({ lobbyId })!");
			}

			int playerId = packet.ReadInt();
			int money = packet.ReadInt();

			//CHECK: send event about money balance of player to everyone
			ServerPacketsSend.ShowMoney(playerId, money, ServerSendHandlers.SendTCPDataToAll);
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
			ServerPacketsSend.ShowDealerButton(playerId, ServerSendHandlers.SendTCPDataToAll);
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
			ServerPacketsSend.GiveCards(playerId, type, suit, ServerSendHandlers.SendTCPData);
			//CHECK: send fake values to everyone except player
			ServerPacketsSend.GiveCards(playerId, (int)CardType.Count, (int)CardSuit.Shirt, ServerSendHandlers.SendTCPDataToAll);
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
			ServerPacketsSend.ShowTableCard(types, suits, indexes, ServerSendHandlers.SendTCPDataToAll);
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

			//CHECK: send event about bet and it's amount to everyone
			ServerPacketsSend.ShowPlayerBet(playerId, betAmount, ServerSendHandlers.SendTCPDataToAll);

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
			ServerPacketsSend.GiveWinAmount(playerId, amount, ServerSendHandlers.SendTCPDataToAll);
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
			ServerPacketsSend.EndTurn(playerId, ServerSendHandlers.SendTCPDataToAll);
		}

		public static void CollectAllBets(int fromLobby, Packet packet)
		{
			int lobbyId = packet.ReadInt();

			if (fromLobby != lobbyId)
			{
				Console.WriteLine($"Lobby (ID: { fromLobby }) has assumed the wrong ID ({ lobbyId })!");
			}

			//CHECK: Send this empty event to all Players in lobbytimeLeft
			ServerPacketsSend.CollectAllBets(ServerSendHandlers.SendTCPDataToAll);
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

			//TODO: send event about real cards to everyone
		}
	}
}
