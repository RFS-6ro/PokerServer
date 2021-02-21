using System;
using System.Collections.Generic;
using Network;
using PokerSynchronisation;

namespace GameServer
{
	public static class MainGameServerSendsToPlayerHandle
	{
		public enum MainGameServerSendsToPlayerTypes
		{
			WelcomeToPlayer = 1,
			LobbyList = 2,
			ConnectionToLobbyApprovance = 3,

			#region Game Loop
			DealerPosition = 4,
			GiveCard = 5,
			ShowTableCards = 6,
			StartTurn = 7,
			TimerEvent = 8,
			TurnApprovance = 9,
			ShowPlayerBet = 10,
			ShowPlayersMoney = 11,
			EndTurn = 12,
			CollectAllBets = 13,
			ShowBank = 14,
			ShowAllCards = 15,
			WinAmount = 16,
			DisconnectFromLobby = 17,
			#endregion
		}

		public static void WelcomeToPlayer(int playerId, string welcomeMessage)
		{
			using (Packet packet = new Packet((int)MainGameServerSendsToPlayerTypes.WelcomeToPlayer))
			{
				packet.Write(playerId);
				packet.Write(welcomeMessage);

				MainGameServerSendToPlayersHandlers.SendTCPData(playerId, packet);
			}
		}

		public static void SendLobbyList(int playerId, List<LobbyIdentifierData> lobbies)
		{
			using (Packet packet = new Packet((int)MainGameServerSendsToPlayerTypes.LobbyList))
			{
				packet.Write(playerId);
				packet.Write(lobbies.Count);

				foreach (var lobby in lobbies)
				{
					packet.Write(lobby.Name);
					packet.Write(lobby.ID);
					packet.Write(lobby.NumberOfPlayers);
					packet.Write(lobby.SmallBlind);
					packet.Write(lobby.BuyIn);
				}

				MainGameServerSendToPlayersHandlers.SendTCPData(playerId, packet);
			}
		}

		public static void DisconnectFromLobby(int playerId)
		{
			using (Packet packet = new Packet((int)MainGameServerSendsToPlayerTypes.DisconnectFromLobby))
			{
				packet.Write(playerId);

				MainGameServerSendToPlayersHandlers.SendTCPData(playerId, packet);
			}
		}

		public static Packet ConnectionToLobbyApprovance(int playerId, string userName, int serverSideIndex)
		{
			using (Packet packet = new Packet((int)MainGameServerSendsToPlayerTypes.ConnectionToLobbyApprovance))
			{
				packet.Write(playerId);
				packet.Write(true);
				packet.Write(userName);
				packet.Write(serverSideIndex);

				return packet;
			}
		}

		public static Packet ConnectionToLobbyApprovance(int playerId, string message)
		{
			using (Packet packet = new Packet((int)MainGameServerSendsToPlayerTypes.ConnectionToLobbyApprovance))
			{
				packet.Write(playerId);
				packet.Write(false);
				packet.Write(message);

				return packet;
			}
		}

		public static void DealerPosition(int dealerId)
		{
			using (Packet packet = new Packet((int)MainGameServerSendsToPlayerTypes.DealerPosition))
			{
				packet.Write(dealerId);

				MainGameServerSendToPlayersHandlers.SendTCPDataToAll(packet);
			}
		}

		public static void GiveCard(int playerId, int type, int suit, Action<int, Packet> sendHandler)
		{
			using (Packet packet = new Packet((int)MainGameServerSendsToPlayerTypes.GiveCard))
			{
				packet.Write(playerId);
				packet.Write(type);
				packet.Write(suit);

				sendHandler(playerId, packet);
			}
		}

		public static void ShowTableCards(int[] types, int[] suits, int[] indexes)
		{
			using (Packet packet = new Packet((int)MainGameServerSendsToPlayerTypes.ShowTableCards))
			{
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

				MainGameServerSendToPlayersHandlers.SendTCPDataToAll(packet);
			}
		}

		public static void StartTurn(int playerId, bool canRaise)
		{
			using (Packet packet = new Packet((int)MainGameServerSendsToPlayerTypes.StartTurn))
			{
				packet.Write(playerId);
				MainGameServerSendToPlayersHandlers.SendTCPDataToAll(playerId, packet);

				packet.Write(canRaise);
				MainGameServerSendToPlayersHandlers.SendTCPData(playerId, packet);
			}
		}

		public static void TimerEvent(int playerId, bool isDecreasing, int timeLeft)
		{
			using (Packet packet = new Packet((int)MainGameServerSendsToPlayerTypes.TimerEvent))
			{
				packet.Write(playerId);
				packet.Write(isDecreasing);
				packet.Write(timeLeft);

				MainGameServerSendToPlayersHandlers.SendTCPDataToAll(packet);
			}
		}

		public static void TurnApprovance(int playerId, bool result)
		{
			using (Packet packet = new Packet((int)MainGameServerSendsToPlayerTypes.TurnApprovance))
			{
				packet.Write(playerId);
				packet.Write(result);

				MainGameServerSendToPlayersHandlers.SendTCPData(playerId, packet);
			}
		}

		public static void ShowPlayerBet(int playerId, int amount, int currentlyInPot)
		{
			using (Packet packet = new Packet((int)MainGameServerSendsToPlayerTypes.ShowPlayerBet))
			{
				packet.Write(playerId);
				packet.Write(amount);
				packet.Write(currentlyInPot);

				MainGameServerSendToPlayersHandlers.SendTCPDataToAll(packet);
			}
		}

		public static void ShowPlayersMoney(int[] playersIds, int[] amounts)
		{
			using (Packet packet = new Packet((int)MainGameServerSendsToPlayerTypes.ShowPlayersMoney))
			{
				packet.Write(playersIds.Length);

				for (int i = 0; i < playersIds.Length; i++)
				{
					packet.Write(playersIds[i]);
					packet.Write(amounts[i]);
				}

				MainGameServerSendToPlayersHandlers.SendTCPDataToAll(packet);
			}
		}

		public static void EndTurn(int playerId)
		{
			using (Packet packet = new Packet((int)MainGameServerSendsToPlayerTypes.EndTurn))
			{
				packet.Write(playerId);

				MainGameServerSendToPlayersHandlers.SendTCPDataToAll(packet);
			}
		}

		public static void CollectAllBets()
		{
			using (Packet packet = new Packet((int)MainGameServerSendsToPlayerTypes.CollectAllBets))
			{
				MainGameServerSendToPlayersHandlers.SendTCPDataToAll(packet);
			}
		}

		public static void ShowBank(int bank)
		{
			using (Packet packet = new Packet((int)MainGameServerSendsToPlayerTypes.ShowBank))
			{
				packet.Write(bank);

				MainGameServerSendToPlayersHandlers.SendTCPDataToAll(packet);
			}
		}

		public static void ShowAllCards(int[] firstCardsTypes, int[] firstCardsSuits, int[] secondCardsTypes, int[] secondCardsSuits, List<int> playersOrderIds)
		{
			using (Packet packet = new Packet((int)MainGameServerSendsToPlayerTypes.ShowAllCards))
			{
				if (firstCardsTypes.Length == firstCardsSuits.Length &&
					firstCardsSuits.Length == secondCardsTypes.Length &&
					secondCardsTypes.Length == secondCardsSuits.Length)
				{
					packet.Write(firstCardsTypes.Length);

					for (int i = 0; i < playersOrderIds.Count; i++)
					{
						packet.Write(playersOrderIds[i]);
						packet.Write(firstCardsTypes[i]);
						packet.Write(firstCardsSuits[i]);
						packet.Write(secondCardsTypes[i]);
						packet.Write(secondCardsSuits[i]);
					}

					MainGameServerSendToPlayersHandlers.SendTCPDataToAll(packet);
				}
			}
		}

		public static void WinAmount(int playerId, int amount)
		{
			using (Packet packet = new Packet((int)MainGameServerSendsToPlayerTypes.WinAmount))
			{
				packet.Write(playerId);
				packet.Write(amount);

				MainGameServerSendToPlayersHandlers.SendTCPDataToAll(packet);
			}
		}
	}
}
