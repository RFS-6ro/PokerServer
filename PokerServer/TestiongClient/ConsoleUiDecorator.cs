using System;
using System.Collections.Generic;
using UniCastCommonData.Packet.InitialDatas;

namespace TestingClient
{
	public class ConsoleUiDecorator
	{
		private int _commonRow = 1;

		private const ConsoleColor PlayerBoxColor = ConsoleColor.DarkGreen;

		private int _row;

		private int _width;

		private bool _isDealer = false;

		private int _type1;
		private int _suit1;
		private int _type2;
		private int _suit2;

		public Guid PlayerGuid { get; set; }

		public string Name { get; set; }

		public ConsoleUiDecorator()
		{

		}

		public void SetEmpty()
		{

		}

		public void SetPlayerData(Guid guid, PlayerData data)
		{

		}

		public void StartGame(StartGameSendingData sendingData)
		{

		}

		public void StartHand(StartHandSendingData sendingData)
		{
			if (Name == sendingData.FirstPlayerName)
			{
				SetDealer();
			}

			var dealerSymbol = _isDealer ? "D" : " ";
			ConsoleHelper.WriteOnConsole(_row + 1, 1, dealerSymbol, ConsoleColor.Green);
			ConsoleHelper.WriteOnConsole(_row + 3, 2, "                            ");
		}

		public void SetDealer()
		{
			_isDealer = true;
		}

		public void SetCards(int type1, int suit1, int type2, int suit2)
		{
			_type1 = type1;
			_suit1 = suit1;
			_type2 = type2;
			_suit2 = suit2;
			DrawSingleCard(_row + 1, 10, type1, suit1);
			DrawSingleCard(_row + 1, 10, type2, suit2);
		}

		public void StartRound(StartRoundSendingData sendingData)
		{

		}

		public void StartTurn(StartTurnSendingData sendingData)
		{

		}

		public void SetTimer(int amount)
		{

		}

		public void ShowTurn(PlayerTurnSendingData sendingData)
		{

			ConsoleHelper.WriteOnConsole(_row + 1, 2, sendingData.MoneyLeft + "   ");

			var action = sendingData.LastPlayerAction;

			if (action == "Fold")
			{
				Muck(sendingData.MoneyLeft);
			}

			ConsoleHelper.WriteOnConsole(_row + 2, 2, new string(' ', _width - 3));

			var lastAction = action;

			if (action == "Call")
			{
				lastAction += $"({sendingData.MoneyToCall})";
			}
			else if (action.Contains("Raise"))
			{
				lastAction += $"({sendingData.LastPlayerActionAmount + sendingData.MoneyInTheRound + sendingData.MoneyToCall})";
			}

			ConsoleHelper.WriteOnConsole(_row + 3, 2, new string(' ', _width - 3));

			if (action == string.Empty)
			{
				return;
			}

			ConsoleHelper.WriteOnConsole(_row + 3, 2, "Last action: " + lastAction);

			var moneyAfterAction = action == "Fold"
				? sendingData.MoneyLeft
				: sendingData.MoneyLeft - sendingData.LastPlayerActionAmount - sendingData.MoneyToCall;

			ConsoleHelper.WriteOnConsole(_row + 1, 2, moneyAfterAction + "   ");
		}

		public void SetMoney(int amount)
		{
			ConsoleHelper.WriteOnConsole(_row + 1, 2, amount.ToString());
		}

		public void EndTurn(EndTurnSendingData sendingData)
		{

		}

		public void EndRound(EndRoundSendingData sendingData)
		{

		}

		public void EndHand(EndHandSendingData sendingData)
		{
			_isDealer = false;
		}

		public void EndGame(EndGameSendingData sendingData)
		{

		}

		private void Muck(int moneyLeft)
		{
			DrawMuckedSingleCard(_row + 1, 10, _type1, _suit1);
			DrawMuckedSingleCard(_row + 1, 14, _type2, _suit2);
		}

		public void DrawGameBox(int row, int width, int commonRow)
		{
			_row = row;
			_width = width;
			_commonRow = commonRow;

			ConsoleHelper.WriteOnConsole(_row, 0, new string('═', _width), PlayerBoxColor);
			ConsoleHelper.WriteOnConsole(_row + 4, 0, new string('═', _width), PlayerBoxColor);
			ConsoleHelper.WriteOnConsole(_row, 0, "╔", PlayerBoxColor);
			ConsoleHelper.WriteOnConsole(_row, _width - 1, "╗", PlayerBoxColor);
			ConsoleHelper.WriteOnConsole(_row + 4, 0, "╚", PlayerBoxColor);
			ConsoleHelper.WriteOnConsole(_row + 4, _width - 1, "╝", PlayerBoxColor);
			for (var i = 1; i < 4; i++)
			{
				ConsoleHelper.WriteOnConsole(_row + i, 0, "║", PlayerBoxColor);
				ConsoleHelper.WriteOnConsole(_row + i, _width - 1, "║", PlayerBoxColor);
			}
		}

		private void DrawSingleCard(int row, int col, int type, int suit)
		{
			var cardColor = ConsoleHelper.GetCardColor(type, suit);
			ConsoleHelper.WriteOnConsole(row, col, " " + ConsoleHelper.ToFriendlyString(type, suit) + " ", cardColor, ConsoleColor.White);
			ConsoleHelper.WriteOnConsole(row, col + 2 + ConsoleHelper.ToFriendlyString(type, suit).Length, " ");
		}

		private void DrawMuckedSingleCard(int row, int col, int type, int suit)
		{
			ConsoleHelper.WriteOnConsole(row, col, " " + ConsoleHelper.ToFriendlyString(type, suit) + " ", ConsoleColor.Gray, ConsoleColor.DarkGray);
		}

	}
}
