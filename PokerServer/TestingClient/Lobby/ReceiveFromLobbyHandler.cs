using System;
using System.Collections.Generic;
using UniCastCommonData;
using UniCastCommonData.Handlers;
using UniCastCommonData.Network.MessageHandlers;
using UniCastCommonData.Packet.InitialDatas;

namespace TestingClient.Lobby.Handlers
{
	public enum lobbyTOclient
	{
		None = 0,

		Connect,

		CurrentGameState,
		NewPlayerConnect,

		StartGame,
		StartHand,
		StartRound,
		StartTurn,

		DealCardsToPlayer,
		DealCardsToTable,

		DealerButton,

		UpdatepPot,
		UpdatePlayersMoney,
		UpdateTimer,

		PlayerTurn,
		OpponentCards,

		EndTurn,
		EndRound,
		EndHand,
		EndGame,

		ClearCards,
		PlayerDisconnect,

		Winners,

		Count,


		Test
	}

	public class ReceiveFromLobbyHandler : ClientReceiveHandlerBase<Client_Lobby>
	{
		public ReceiveFromLobbyHandler()
		{
			Handlers.Add((int)lobbyTOclient.Test, Test);

			Handlers.Add((int)lobbyTOclient.Connect, Connect);

			Handlers.Add((int)lobbyTOclient.CurrentGameState, CurrentGameState);
			Handlers.Add((int)lobbyTOclient.NewPlayerConnect, NewPlayerConnect);

			Handlers.Add((int)lobbyTOclient.StartGame, StartGame);
			Handlers.Add((int)lobbyTOclient.StartHand, StartHand);
			Handlers.Add((int)lobbyTOclient.StartRound, StartRound);
			Handlers.Add((int)lobbyTOclient.StartTurn, StartTurn);

			Handlers.Add((int)lobbyTOclient.DealCardsToPlayer, DealCardsToPlayer);
			Handlers.Add((int)lobbyTOclient.DealCardsToTable, DealCardsToTable);

			Handlers.Add((int)lobbyTOclient.DealerButton, DealerButton);

			Handlers.Add((int)lobbyTOclient.UpdatepPot, UpdatepPot);
			Handlers.Add((int)lobbyTOclient.UpdatePlayersMoney, UpdatePlayersMoney);
			Handlers.Add((int)lobbyTOclient.UpdateTimer, UpdateTimer);

			Handlers.Add((int)lobbyTOclient.PlayerTurn, PlayerTurn);
			Handlers.Add((int)lobbyTOclient.OpponentCards, OpponentCards);

			Handlers.Add((int)lobbyTOclient.EndTurn, EndTurn);
			Handlers.Add((int)lobbyTOclient.EndRound, EndRound);
			Handlers.Add((int)lobbyTOclient.EndHand, EndHand);
			Handlers.Add((int)lobbyTOclient.EndGame, EndGame);

			Handlers.Add((int)lobbyTOclient.ClearCards, ClearCards);
			Handlers.Add((int)lobbyTOclient.PlayerDisconnect, PlayerDisconnect);

			Handlers.Add((int)lobbyTOclient.Winners, Winners);
		}

		private void PlayerDisconnect(UniCastPacket packet)
		{
			var sendingData = new PlayerDisconnectSendingData(packet.GetRawBytes());

			PokerInitializer.Instance.RemovePlayer(sendingData.Player);
		}

		private void ClearCards(UniCastPacket packet)
		{
			var sendingData = new ClearCardsSendingData(packet.GetRawBytes());

			PokerInitializer initializer = PokerInitializer.Instance;

			if (sendingData.CardKeeper != Guid.Empty)
			{
				ConsoleUiDecorator player = initializer.FindPlayerByGuid(sendingData.CardKeeper);
				player.SetCards(-1, -1, -1, -1);
				return;
			}

			//clear all cards;
			foreach (var player in initializer.Decorators)
			{
				player.SetCards(-1, -1, -1, -1);
			}

			initializer.Table.ClearCards();
		}

		private void EndGame(UniCastPacket packet)
		{
			var sendingData = new EndGameSendingData(packet.GetRawBytes());

			PokerInitializer.Instance.FindPlayerByGuid(sendingData.ReceiverGuid).EndGame(sendingData);
		}

		private void EndHand(UniCastPacket packet)
		{
			var sendingData = new EndHandSendingData(packet.GetRawBytes());

			PokerInitializer.Instance.FindPlayerByGuid(sendingData.ReceiverGuid).EndHand(sendingData);
		}

		private void EndRound(UniCastPacket packet)
		{
			var sendingData = new EndRoundSendingData(packet.GetRawBytes());
			PokerInitializer.Instance.FindPlayerByGuid(sendingData.ReceiverGuid).EndRound(sendingData);
		}

		private void EndTurn(UniCastPacket packet)
		{
			var sendingData = new EndTurnSendingData(packet.GetRawBytes());
			PokerInitializer.Instance.FindPlayerByGuid(sendingData.Player).EndTurn(sendingData);
		}

		private void OpponentCards(UniCastPacket packet)
		{
			var sendingData = new OpponentCardsSendingData(packet.GetRawBytes());

			foreach (var playerCards in sendingData.Cards)
			{
				PokerInitializer.Instance.
					FindPlayerByGuid(playerCards.Key).
					SetCards(playerCards.Value.Item1,
							 playerCards.Value.Item2,
							 playerCards.Value.Item3,
							 playerCards.Value.Item4);
			}
		}

		private void PlayerTurn(UniCastPacket packet)
		{
			var sendingData = new PlayerTurnSendingData(packet.GetRawBytes());

			PokerInitializer.Instance.FindPlayerByGuid(sendingData.Player).ShowTurn(sendingData);
			PokerInitializer.Instance.FindPlayerByGuid(Guid.Empty).SetDataToMakeTurn(sendingData.MinRaise, sendingData.MaxBet);

		}

		private void UpdateTimer(UniCastPacket packet)
		{
			var sendingData = new UpdateTimerSendingData(packet.GetRawBytes());

			PokerInitializer.Instance.FindPlayerByGuid(sendingData.Player).SetTimer(sendingData.Milliseconds);
		}

		private void UpdatePlayersMoney(UniCastPacket packet)
		{
			var sendingData = new UpdatePlayersMoneySendingData(packet.GetRawBytes());

			foreach (var playerMoney in sendingData.Moneys)
			{
				PokerInitializer.Instance.
					FindPlayerByGuid(playerMoney.Key).
					SetMoney(playerMoney.Value);
			}
		}

		private void UpdatepPot(UniCastPacket packet)
		{
			var sendingData = new UpdatePotSendingData(packet.GetRawBytes());

			PokerInitializer.Instance.Table.ShowPot(sendingData.Pot);
		}

		private void DealerButton(UniCastPacket packet)
		{
			var sendingData = new DealerButtonSendingData(packet.GetRawBytes());

			ConsoleUiDecorator player = PokerInitializer.Instance.FindPlayerByGuid(sendingData.Dealer);
			player.SetDealer();
		}

		private void DealCardsToTable(UniCastPacket packet)
		{
			var sendingData = new DealCardsToTableSendingData(packet.GetRawBytes());

			PokerInitializer.Instance.Table.AddCards(sendingData.Cards);
		}

		private void Winners(UniCastPacket packet)
		{
			var sendingData = new WinnersSendingData(packet.GetRawBytes());

			PokerInitializer.Instance.HighlightWinners(sendingData.Winners);
		}

		private void DealCardsToPlayer(UniCastPacket packet)
		{
			var sendingData = new DealCardsToPlayerSendingData(packet.GetRawBytes());

			List<ConsoleUiDecorator> players = PokerInitializer.Instance.Decorators;
			foreach (var player in players)
			{
				if (player.PlayerGuid == sendingData.ReceiverGuid)
				{
					player.SetCards(sendingData.CardType1,
									sendingData.CardSuit1,
									sendingData.CardType2,
									sendingData.CardSuit2);
				}
				else
				{
					player.SetCards(15, 4, 15, 4);
				}
			}

		}

		private void StartTurn(UniCastPacket packet)
		{
			var sendingData = new StartTurnSendingData(packet.GetRawBytes());

			PokerInitializer.Instance.FindPlayerByGuid(sendingData.Player).StartTurn(sendingData);
		}

		private void StartGame(UniCastPacket packet)
		{
			var sendingData = new StartGameSendingData(packet.GetRawBytes());
			PokerInitializer.Instance.FindPlayerByGuid(sendingData.ReceiverGuid).StartGame(sendingData);
		}

		private void StartRound(UniCastPacket packet)
		{
			var sendingData = new StartRoundSendingData(packet.GetRawBytes());

			PokerInitializer.Instance.FindPlayerByGuid(sendingData.ReceiverGuid).StartRound(sendingData);
			PokerInitializer.Instance.Table.ShowPot(sendingData.Pot);
		}

		private void StartHand(UniCastPacket packet)
		{
			var sendingData = new StartHandSendingData(packet.GetRawBytes());

			PokerInitializer.Instance.FindPlayerByGuid(sendingData.ReceiverGuid).StartHand(sendingData);
			PokerInitializer.Instance.Table.DrawCommunityCards();
			PokerInitializer.Instance.Table.ShowPot(0);
		}

		private void NewPlayerConnect(UniCastPacket packet)
		{
			var sendingData = new NewPlayerConnectSendingData(packet.GetRawBytes());
			PokerInitializer.Instance.AddNewPlayer(sendingData);
		}

		private void CurrentGameState(UniCastPacket packet)
		{
			var sendingData = new CurrentGameStateSendingData(packet.GetRawBytes());
			PokerInitializer.Instance.SetCurrentGameState(sendingData);
		}

		private void Connect(UniCastPacket packet)
		{
			Guid receiverGuid = new Guid(packet.Read(16));
			Client_Lobby.Instance.SetId(ReceiverId);
		}

		private void Test(UniCastPacket packet)
		{
			Guid receiverGuid = new Guid(packet.Read(16));
			string message = packet.ReadString();
		}
	}
}