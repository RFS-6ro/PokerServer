using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TestingClient.Lobby;
using TestingClient.Lobby.Handlers;
using UniCastCommonData;
using UniCastCommonData.Handlers;
using UniCastCommonData.Packet.InitialDatas;

namespace TestingClient
{
	public class ConsoleUiDecorator
	{
		private ConsoleColor PlayerBoxColor = ConsoleColor.DarkGreen;

		private int _row;

		private int _width;

		private bool _isDealer = false;

		private int _type1;
		private int _suit1;
		private int _type2;
		private int _suit2;

		private bool _isPlaying = false;

		public int Time = 0;
		private bool _isRealPlayer = false;

		public Guid PlayerGuid { get; set; }

		public string Name { get; set; }

		public bool CanRaise => MinRaise > 0 && MoneyLeft > MoneyToCall;

		public int MoneyToCall
		{
			get
			{
				var temp = CurrentMaxBet - MyMoneyInTheRound;
				if (temp >= MoneyLeft)
				{
					// The player does not have enough money to make a full call
					return MoneyLeft;
				}
				else
				{
					return temp;
				}
			}
		}

		public int CurrentMaxBet { get; set; }

		public int MyMoneyInTheRound { get; set; }//

		public int MoneyLeft { get; set; }//

		public int MinRaise { get; set; }

		public ConsoleUiDecorator(int row, int width)
		{
			_row = row;
			_width = width;
			SetEmpty();
		}

		public void SetEmpty()
		{
			Time = 0;
			Name = string.Empty;
			PlayerGuid = Guid.Empty;
			_isRealPlayer = false;
			SetBet(0);
			SetMoney(-1);
			SetDealer(false);
			SetCards(-1, -1, -1, -1);
			DrawGameBox();
		}

		public void SetPlayerData(Guid guid, PlayerData data)
		{
			if (guid == Guid.Empty)
			{
				SetEmpty();
				return;
			}

			PlayerGuid = guid;
			Name = data.Name;

			_isRealPlayer = guid == IStaticInstance<Client_Lobby>.Instance.Id;

			SetDealer(data.IsDealer);
			DrawGameBox();

			if (data.InGame)
			{
				SetCards(15, 4, 15, 4);
				SetBet(data.Bet);
			}

			SetMoney(data.Money);

			ConsoleHelper.WriteOnConsole(_row + 3, 2, new string(' ', _width - 3));

			if (data.LastPlayerAction == string.Empty)
			{
				return;
			}

			ConsoleHelper.WriteOnConsole(_row + 3, 2, "Last action: " + data.LastPlayerAction);
		}

		public void StartGame(StartGameSendingData sendingData)
		{
			_isPlaying = true;
			SetBet(0);
			DrawGameBox();
			SetMoney(sendingData.StartMoney);
		}

		public void StartHand(StartHandSendingData sendingData)
		{
			SetBet(0);
			SetDealer(Name == sendingData.FirstPlayerName);

			DrawGameBox();
		}

		public void SetDealer(bool value = true)
		{
			_isDealer = value;

			var dealerSymbol = _isDealer ? "D" : " ";
			ConsoleHelper.WriteOnConsole(_row + 1, 1, dealerSymbol, ConsoleColor.Green);
			ConsoleHelper.WriteOnConsole(_row + 3, 2, "                            ");
		}

		public void SetCards(int type1, int suit1, int type2, int suit2)
		{
			_type1 = type1;
			_suit1 = suit1;
			_type2 = type2;
			_suit2 = suit2;
			DrawSingleCard(_row + 1, 10, type1, suit1);
			DrawSingleCard(_row + 1, 14, type2, suit2);
		}

		public void StartRound(StartRoundSendingData sendingData)
		{
			SetBet(0);
			DrawGameBox();
			SetMoney(sendingData.Money);
		}

		public void StartTurn(StartTurnSendingData sendingData)
		{
			if (_isRealPlayer == false)
			{
				return;
			}

			CurrentMaxBet = sendingData.MaxMoneyPerPlayer;
			MyMoneyInTheRound = sendingData.MyMoneyInTheRound;
			MoneyLeft = sendingData.Money;
			MinRaise = sendingData.MinRaise;

			DrawGameBox();

			InputModel.OnTurnSetted += OnTurnSetted;
		}

		private void OnTurnSetted()
		{
			//TODOSEND: player input
			IStaticInstance<Client_Lobby>.
				Instance.
				SendHandler.
				SendAsync(new PlayerInputSendingData(
						InputModel.GetTurn(),
						Guid.Empty,
						IStaticInstance<Client_Lobby>.Instance.Id,
						ActorType.Client,
						(int)clientTOlobby.SendTurn), null);
		}

		public void SetBet(int amount)
		{
			MyMoneyInTheRound = amount < 0 ? 0 : amount;
			string bet = amount <= 0 ? "      " : amount.ToString();

			ConsoleHelper.WriteOnConsole(_row, 20, $" Bet = {bet} ", ConsoleColor.Black, ConsoleColor.White);
		}

		public void SetTimer(int amount)
		{
			Time = amount;
			DrawGameBox();
		}

		public void SetWinner(int prize, string handRank)
		{
			PlayerBoxColor = ConsoleColor.DarkYellow;
			DrawGameBox();

			//write prize and hand
			ConsoleHelper.WriteOnConsole(_row + 3, 2, $"Winner: {prize} / {handRank}");

			PlayerBoxColor = ConsoleColor.DarkGreen;
		}

		public void ResetWinner()
		{
			PlayerBoxColor = ConsoleColor.DarkGreen;
			DrawGameBox();

			//delete prize and hand
			ConsoleHelper.WriteOnConsole(_row + 3, 2, new string(' ', _width));
		}

		public void SetDataToMakeTurn(int minRaise, int currentMaxBet)
		{
			MinRaise = minRaise;
			CurrentMaxBet = currentMaxBet;
		}

		public void ShowTurn(PlayerTurnSendingData sendingData)
		{
			if (sendingData.Player == PlayerGuid)
			{
				InputModel.OnTurnSetted -= OnTurnSetted;
			}

			int betAmount = -1;
			DrawGameBox();

			SetMoney(sendingData.MoneyLeft);

			var action = sendingData.LastPlayerAction;

			if (action == "Fold")
			{
				Muck();
			}

			ConsoleHelper.WriteOnConsole(_row + 2, 2, new string(' ', _width - 3));

			var lastAction = action;

			if (action == "Call")
			{
				betAmount = sendingData.MoneyToCall;
				lastAction += $"({betAmount})";
			}
			else if (action.Contains("Raise"))
			{
				betAmount = sendingData.LastPlayerActionAmount + sendingData.MoneyInTheRound + sendingData.MoneyToCall;
			}
			else if (action.Contains("Post"))
			{
				betAmount = sendingData.LastPlayerActionAmount;
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

			SetBet(betAmount);

			SetMoney(moneyAfterAction);
		}

		public void SetMoney(int amount)
		{
			MoneyLeft = amount < 0 ? 0 : amount;
			string money = amount < 0 ? "         " : amount.ToString();
			ConsoleHelper.WriteOnConsole(_row + 1, 2, money + "   ");
		}

		public void EndTurn(EndTurnSendingData sendingData)
		{
			Time = 0;
			DrawGameBox();
		}

		public void EndRound(EndRoundSendingData sendingData)
		{
			SetBet(0);
			DrawGameBox();
		}

		public void EndHand(EndHandSendingData sendingData)
		{
			SetBet(0);
			ResetWinner();
			SetDealer(false);
			DrawGameBox();
		}

		public void EndGame(EndGameSendingData sendingData)
		{
			_isPlaying = false;
			DrawGameBox();
		}

		private void Muck()
		{
			SetBet(0);
			DrawMuckedSingleCard(_row + 1, 10, _type1, _suit1);
			DrawMuckedSingleCard(_row + 1, 14, _type2, _suit2);
		}

		public void DrawGameBox()
		{
			string top;
			if (Name != null && Name != string.Empty)
			{
				if (_isRealPlayer)
				{
					top = new string(new char[] { '═', 'Y', 'O', 'U', '═' });
				}
				else
				{
					top = new string('═', 5);
				}

				top += Name + "═";
				if (Time != 0)
				{
					string time = Time.ToString();
					top += time + new string('═', 5 - time.Length);
				}
				else
				{
					top += new string('═', 4);
				}
			}
			else
			{
				top = new string('═', _width);
			}
			ConsoleHelper.WriteOnConsole(_row, 0, top, PlayerBoxColor);
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

			if (_isRealPlayer && _isPlaying)
			{
				if (!CanRaise)
				{
					DrawRestrictedPlayerOptions(MoneyToCall);
				}
				else
				{
					DrawPlayerOptions(MoneyToCall);
				}
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

		private void DrawPlayerOptions(int moneyToCall)
		{
			var col = 2;
			ConsoleHelper.WriteOnConsole(_row + 2, col, "Select action: [");
			col += 16;
			ConsoleHelper.WriteOnConsole(_row + 2, col, "C", ConsoleColor.Yellow);
			col++;
			ConsoleHelper.WriteOnConsole(_row + 2, col, "]heck/[");
			col += 7;
			ConsoleHelper.WriteOnConsole(_row + 2, col, "C", ConsoleColor.Yellow);
			col++;

			var callString = moneyToCall <= 0 ? "]all, [" : "]all(" + moneyToCall + "), [";

			ConsoleHelper.WriteOnConsole(_row + 2, col, callString);
			col += callString.Length;
			ConsoleHelper.WriteOnConsole(_row + 2, col, "R", ConsoleColor.Yellow);
			col++;
			ConsoleHelper.WriteOnConsole(_row + 2, col, "]aise, [");
			col += 8;
			ConsoleHelper.WriteOnConsole(_row + 2, col, "F", ConsoleColor.Yellow);
			col++;
			ConsoleHelper.WriteOnConsole(_row + 2, col, "]old, [");
			col += 7;
			ConsoleHelper.WriteOnConsole(_row + 2, col, "A", ConsoleColor.Yellow);
			col++;
			ConsoleHelper.WriteOnConsole(_row + 2, col, "]ll-in");
		}

		private void DrawRestrictedPlayerOptions(int moneyToCall)
		{
			var col = 2;
			ConsoleHelper.WriteOnConsole(_row + 2, col, "Select action: [");
			col += 16;
			ConsoleHelper.WriteOnConsole(_row + 2, col, "C", ConsoleColor.Yellow);
			col++;

			var callString = moneyToCall <= 0 ? "]all, [" : "]all(" + moneyToCall + "), [";

			ConsoleHelper.WriteOnConsole(_row + 2, col, callString);
			col += callString.Length;
			ConsoleHelper.WriteOnConsole(_row + 2, col, "F", ConsoleColor.Yellow);
			col++;
			ConsoleHelper.WriteOnConsole(_row + 2, col, "]old");
		}
	}
}
