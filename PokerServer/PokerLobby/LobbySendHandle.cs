using System;
using System.Collections.Generic;
using Network;
using TexasHoldem.Logic.Cards;

namespace PokerLobby
{
	public static class LobbySendHandle
	{
		public enum LobbySendTypes
		{
			WelcomeReceived = 1,
			ConnectionToLobbyApprovance,

			#region Game Loop
			DealerPosition,
			GiveCard,
			ShowTableCards,
			StartTurn,
			TimerEvent,
			TurnApprovance,
			ShowPlayerBet,
			ShowPlayerMoney,
			EndTurn,
			CollectAllBets,
			ShowBank,
			ShowAllCards,
			WinAmount,
			#endregion
		}

		public static void WelcomeReceived(int id, string name)
		{
			using (Packet packet = new Packet((int)LobbySendTypes.WelcomeReceived))
			{
				packet.Write(id);
				packet.Write(name);

				ClientSentHandlers.SendTCPData(packet);
			}
		}

		public static void ConnectionToLobbyApprovance(int connectedLobbyId, int connectedPlayerId)
		{
			using (Packet packet = new Packet((int)LobbySendTypes.ConnectionToLobbyApprovance))
			{
				packet.Write(connectedLobbyId);
				packet.Write(connectedPlayerId);
				packet.Write(true);

				ClientSentHandlers.SendTCPData(packet);
			}
		}

		public static void ConnectionToLobbyApprovance(int connectedLobbyId, int connectedPlayerId, string message)
		{
			using (Packet packet = new Packet((int)LobbySendTypes.ConnectionToLobbyApprovance))
			{
				packet.Write(connectedLobbyId);
				packet.Write(connectedPlayerId);
				packet.Write(false);
				packet.Write(message);

				ClientSentHandlers.SendTCPData(packet);
			}
		}

		public static void DealerPosition(int lobbyId, int dealerPlayerId)
		{
			using (Packet packet = new Packet((int)LobbySendTypes.DealerPosition))
			{
				packet.Write(lobbyId);
				packet.Write(dealerPlayerId);

				ClientSentHandlers.SendTCPData(packet);
			}
		}

		public static void GiveCard(int lobbyId, int playerId, int type, int suit)
		{
			using (Packet packet = new Packet((int)LobbySendTypes.GiveCard))
			{
				packet.Write(lobbyId);
				packet.Write(playerId);
				packet.Write(type);
				packet.Write(suit);

				ClientSentHandlers.SendTCPData(packet);
			}
		}

		public static void ShowTableCards(int lobbyId, int[] types, int[] suits, int[] indexes)
		{
			using (Packet packet = new Packet((int)LobbySendTypes.ShowTableCards))
			{
				packet.Write(lobbyId);

				packet.Write(types.Length);
				for (int i = 0; i < types.Length; i++)
				{
					int type = types[i];
					int suit = suits[i];
					int index = indexes[i];

					packet.Write(type);
					packet.Write(suit);
					packet.Write(index);
				}

				ClientSentHandlers.SendTCPData(packet);
			}
		}

		public static void StartTurn(int lobbyId, int playerId, bool canRaise)
		{
			using (Packet packet = new Packet((int)LobbySendTypes.StartTurn))
			{
				packet.Write(lobbyId);
				packet.Write(playerId);
				packet.Write(canRaise);

				ClientSentHandlers.SendTCPData(packet);
			}
		}

		public static void TimerEvent(int lobbyId, int playerId, bool isDecreasing, int timeLeft)
		{
			using (Packet packet = new Packet((int)LobbySendTypes.TimerEvent))
			{
				packet.Write(lobbyId);
				packet.Write(playerId);
				packet.Write(isDecreasing);
				packet.Write(timeLeft);

				ClientSentHandlers.SendTCPData(packet);
			}
		}

		public static void TurnApprovance(int lobbyId, int playerId, bool result)
		{
			using (Packet packet = new Packet((int)LobbySendTypes.TurnApprovance))
			{
				packet.Write(lobbyId);
				packet.Write(playerId);
				packet.Write(result);

				ClientSentHandlers.SendTCPData(packet);
			}
		}

		public static void ShowPlayerBet(int lobbyId, int playerId, int betAmount)
		{
			using (Packet packet = new Packet((int)LobbySendTypes.ShowPlayerBet))
			{
				packet.Write(lobbyId);
				packet.Write(playerId);
				packet.Write(betAmount);

				ClientSentHandlers.SendTCPData(packet);
			}
		}

		public static void ShowPlayerMoney(int lobbyId, int playerId, int amount)
		{
			using (Packet packet = new Packet((int)LobbySendTypes.ShowPlayerMoney))
			{
				packet.Write(lobbyId);
				packet.Write(playerId);
				packet.Write(amount);

				ClientSentHandlers.SendTCPData(packet);
			}
		}

		public static void EndTurn(int lobbyId, int playerId)
		{
			using (Packet packet = new Packet((int)LobbySendTypes.EndTurn))
			{
				packet.Write(lobbyId);
				packet.Write(playerId);

				ClientSentHandlers.SendTCPData(packet);
			}
		}

		public static void CollectAllBets(int lobbyId)
		{
			using (Packet packet = new Packet((int)LobbySendTypes.CollectAllBets))
			{
				packet.Write(lobbyId);

				ClientSentHandlers.SendTCPData(packet);
			}
		}

		public static void ShowBank(int lobbyId, int amount)
		{
			using (Packet packet = new Packet((int)LobbySendTypes.ShowBank))
			{
				packet.Write(lobbyId);
				packet.Write(amount);

				ClientSentHandlers.SendTCPData(packet);
			}
		}

		public static void ShowAllCards(int lobbyId, List<Card> firstCards, List<Card> secondCards, List<int> playersOrderIds)
		{
			using (Packet packet = new Packet((int)LobbySendTypes.ShowAllCards))
			{
				if (firstCards.Count == secondCards.Count &&
					firstCards.Count == playersOrderIds.Count)
				{
					packet.Write(lobbyId);
					packet.Write(firstCards.Count);

					for (int i = 0; i < playersOrderIds.Count; i++)
					{
						packet.Write(playersOrderIds[i]);
						packet.Write((int)firstCards[i].Type);
						packet.Write((int)firstCards[i].Suit);
						packet.Write((int)secondCards[i].Type);
						packet.Write((int)secondCards[i].Suit);
					}

					ClientSentHandlers.SendTCPData(packet);
				}
			}
		}

		public static void WinAmount(int lobbyId, int playerId, int amount)
		{
			using (Packet packet = new Packet((int)LobbySendTypes.WinAmount))
			{
				packet.Write(lobbyId);
				packet.Write(playerId);
				packet.Write(amount);

				ClientSentHandlers.SendTCPData(packet);
			}
		}
	}
}