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
		}

		private void PlayerDisconnect(UniCastPacket packet)
		{
			var sendingData = new PlayerDisconnectSendingData(packet.GetRawBytes());

			ConsoleUiDecorator player = IStaticInstance<PokerInitializer>.Instance.RemovePlayer(sendingData.Player);
		}

		private void ClearCards(UniCastPacket packet)
		{
			var sendingData = new ClearCardsSendingData(packet.GetRawBytes());

			if (sendingData.CardKeeper == Guid.Empty)
			{
				//TODO: clear all cards;
			}
			else
			{
				ConsoleUiDecorator player = IStaticInstance<PokerInitializer>.Instance.FindPlayerByGuid(sendingData.CardKeeper);
				player.SetCards(-1, -1, -1, -1);
			}
		}

		private void EndGame(UniCastPacket packet)
		{
			var sendingData = new EndGameSendingData(packet.GetRawBytes());


		}

		private void EndHand(UniCastPacket packet)
		{
			var sendingData = new EndHandSendingData(packet.GetRawBytes());


		}

		private void EndRound(UniCastPacket packet)
		{
			var sendingData = new EndRoundSendingData(packet.GetRawBytes());


		}

		private void EndTurn(UniCastPacket packet)
		{
			var sendingData = new EndTurnSendingData(packet.GetRawBytes());


		}

		private void OpponentCards(UniCastPacket packet)
		{
			var sendingData = new OpponentCardsSendingData(packet.GetRawBytes());

			foreach (var playerCards in sendingData.Cards)
			{
				IStaticInstance<PokerInitializer>.Instance.
					FindPlayerByGuid(playerCards.Key).
					SetCards(playerCards.Value.type1,
							 playerCards.Value.suit1,
							 playerCards.Value.type2,
							 playerCards.Value.suit2);
			}
		}

		private void PlayerTurn(UniCastPacket packet)
		{
			var sendingData = new PlayerTurnSendingData(packet.GetRawBytes());


		}

		private void UpdateTimer(UniCastPacket packet)
		{
			var sendingData = new UpdateTimerSendingData(packet.GetRawBytes());


		}

		private void UpdatePlayersMoney(UniCastPacket packet)
		{
			var sendingData = new UpdatePlayersMoneySendingData(packet.GetRawBytes());

			foreach (var playerMoney in sendingData.Moneys)
			{
				IStaticInstance<PokerInitializer>.Instance.
					FindPlayerByGuid(playerMoney.Key).
					SetMoney(playerMoney.Value);
			}
		}

		private void UpdatepPot(UniCastPacket packet)
		{
			var sendingData = new UpdatePotSendingData(packet.GetRawBytes());

			IStaticInstance<PokerInitializer>.Instance.Table.ShowPot(sendingData.Pot);
		}

		private void DealerButton(UniCastPacket packet)
		{
			var sendingData = new DealerButtonSendingData(packet.GetRawBytes());

			ConsoleUiDecorator player = IStaticInstance<PokerInitializer>.Instance.FindPlayerByGuid(sendingData.Dealer);
			player.SetDealer();
		}

		private void DealCardsToTable(UniCastPacket packet)
		{
			var sendingData = new DealCardsToTableSendingData(packet.GetRawBytes());

			IStaticInstance<PokerInitializer>.Instance.Table.AddCards(sendingData.Cards);
		}

		private void DealCardsToPlayer(UniCastPacket packet)
		{
			var sendingData = new DealCardsToPlayerSendingData(packet.GetRawBytes());

			ConsoleUiDecorator player = IStaticInstance<PokerInitializer>.Instance.FindPlayerByGuid(sendingData.ReceiverGuid);
			player.SetCards(sendingData.CardType1,
							sendingData.CardSuit1,
							sendingData.CardType2,
							sendingData.CardSuit2);
		}

		private void StartTurn(UniCastPacket packet)
		{
			var sendingData = new StartTurnSendingData(packet.GetRawBytes());


		}

		private void StartGame(UniCastPacket packet)
		{
			var sendingData = new StartGameSendingData(packet.GetRawBytes());
			ConsoleUiDecorator player = IStaticInstance<PokerInitializer>.Instance.FindPlayerByGuid(sendingData.ReceiverGuid);
			player.SetMoney(sendingData.StartMoney);
		}

		private void StartRound(UniCastPacket packet)
		{
			var sendingData = new StartRoundSendingData(packet.GetRawBytes());


		}

		private void StartHand(UniCastPacket packet)
		{
			var sendingData = new StartHandSendingData(packet.GetRawBytes());

			ConsoleUiDecorator player = IStaticInstance<PokerInitializer>.Instance.FindPlayerByGuid(sendingData.ReceiverGuid);
			if (player.Name == sendingData.FirstPlayerName)
			{
				player.SetDealer();
			}
			IStaticInstance<PokerInitializer>.Instance.Table.DrawCommunityCards();
			IStaticInstance<PokerInitializer>.Instance.Table.ShowPot(0);
		}

		private void NewPlayerConnect(UniCastPacket packet)
		{
			var sendingData = new NewPlayerConnectSendingData(packet.GetRawBytes());

			ConsoleUiDecorator newPlayer = IStaticInstance<PokerInitializer>.Instance.AddNewPlayer(sendingData);
		}

		private void CurrentGameState(UniCastPacket packet)
		{
			var sendingData = new CurrentGameStateSendingData(packet.GetRawBytes());
			IStaticInstance<PokerInitializer>.Instance.SetCurrentGameState(sendingData);
		}

		private void Connect(UniCastPacket packet)
		{
			Guid receiverGuid = new Guid(packet.Read(16));
			Client.SetId(ReceiverId);
		}

		private void Test(UniCastPacket packet)
		{
			Guid receiverGuid = new Guid(packet.Read(16));
			string message = packet.ReadString();
		}
	}
}