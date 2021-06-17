using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LobbyServer.Client;
using LobbyServer.pokerlogic.Cards;
using LobbyServer.pokerlogic.Extensions;
using LobbyServer.pokerlogic.pokermodel.Players;
using PokerSynchronisation;
using UniCastCommonData;
using UniCastCommonData.Network.MessageHandlers;

namespace LobbyServer.pokerlogic.pokermodel.UI
{
	public class ConsoleUiDecorator : PlayerDecorator
	{
		public Guid PlayerGuid { get; protected set; }

		private ConsoleColor PlayerBoxColor = ConsoleColor.DarkGreen;

		private int _row;

		private int _width;

		private Card firstCard;

		private Card secondCard;

		private Lobby_Client_Server Server => IStaticInstance<Lobby_Client_Server>.Instance;
		private SessionSender<Lobby_Client_Server> Sender => IStaticInstance<Lobby_Client_Server>.Instance.SendHandler;

		public int Time = 0;
		public bool IsDealer;

		public ConsoleUiDecorator(int row, int width)
		{
			_row = row;
			_width = width;
		}

		public override void SetPlayer(IPlayer player)
		{
			base.SetPlayer(player);

			if (player.GetType() == typeof(ServerPlayer))
			{
				PlayerGuid = ((ServerPlayer)player).Guid;
				Name = player.Name;
			}
		}

		public override void StartHand(IStartHandContext context)
		{
			IsDealer = context.FirstPlayerName == Player.Name;
			var dealerSymbol = context.FirstPlayerName == Player.Name ? "D" : " ";

			ConsoleHelper.WriteOnConsole(_row + 1, 1, dealerSymbol, ConsoleColor.Green);
			ConsoleHelper.WriteOnConsole(_row + 3, 2, "                            ");

			ConsoleHelper.WriteOnConsole(_row + 1, 2, context.MoneyLeft.ToString());

			base.StartHand(context);
		}

		public override void AddCard(Card card, bool isFirst)
		{
			base.AddCard(card, isFirst);

			if (isFirst)
			{
				firstCard = card.DeepClone();
				DrawSingleCard(_row + 1, 10, firstCard);
			}
			else
			{
				secondCard = card.DeepClone();
				DrawSingleCard(_row + 1, 14, secondCard);
			}
		}

		public override void StartRound(IStartRoundContext context)
		{
			ConsoleHelper.WriteOnConsole(_row + 1, _width - 11, context.RoundType + "   ");
			ConsoleHelper.WriteOnConsole(_row + 3, 2, new string(' ', _width - 3));
			base.StartRound(context);
		}

		public override PlayerAction PostingBlind(IPostingBlindContext context)
		{
			var action = base.PostingBlind(context);

			ConsoleHelper.WriteOnConsole(_row + 2, 2, new string(' ', _width - 3));
			ConsoleHelper.WriteOnConsole(_row + 3, 2, "Last action: " + action.Type + "(" + action.Money + ")");

			var moneyAfterAction = context.CurrentStackSize;

			ConsoleHelper.WriteOnConsole(_row + 1, 2, moneyAfterAction + "   ");

			return action;
		}

		public async override Task<PlayerAction> GetTurn(IGetTurnContext context)
		{
			ConsoleHelper.WriteOnConsole(_row + 1, 2, context.MoneyLeft + "   ");

			var action = await base.GetTurn(context);

			if (action.Type == TurnType.Fold)
			{
				Muck();
			}

			ConsoleHelper.WriteOnConsole(_row + 2, 2, new string(' ', _width - 3));

			var lastAction = action.Type.ToString();

			if (action.Type == TurnType.Call)
			{
				lastAction += $"({context.MoneyToCall})";
			}
			else if (action.Type == TurnType.Raise)
			{
				lastAction += $"({action.Money + context.MyMoneyInTheRound + context.MoneyToCall})";
			}

			ConsoleHelper.WriteOnConsole(_row + 3, 2, new string(' ', _width - 3));
			ConsoleHelper.WriteOnConsole(_row + 3, 2, "Last action: " + lastAction);

			var moneyAfterAction = action.Type == TurnType.Fold
				? context.MoneyLeft
				: context.MoneyLeft - action.Money - context.MoneyToCall;

			ConsoleHelper.WriteOnConsole(_row + 1, 2, moneyAfterAction + "   ");

			return action;
		}

		private void DrawSingleCard(int row, int col, Card card)
		{
			var cardColor = GetCardColor(card);
			ConsoleHelper.WriteOnConsole(row, col, " " + card + " ", cardColor, ConsoleColor.White);
			ConsoleHelper.WriteOnConsole(row, col + 2 + card.ToString().Length, " ");
		}

		internal void Disconnect()
		{
			throw new NotImplementedException();
		}

		public async override Task<PlayerAction> AwaitTurn(IGetTurnContext context)
		{
			PlayerAction action = await GetTurn(context);

			return action;
		}

		public void SetWinner(int prize, string handRank)
		{
			PlayerBoxColor = ConsoleColor.DarkYellow;
			DrawGameBox();

			//write prize and hand
			ConsoleHelper.WriteOnConsole(_row + 3, 2, $"Winner: prize:{prize} with hand rank: {handRank}");

			PlayerBoxColor = ConsoleColor.DarkGreen;
		}

		public void ResetWinner()
		{
			PlayerBoxColor = ConsoleColor.DarkGreen;
			DrawGameBox();

			//delete prize and hand
			ConsoleHelper.WriteOnConsole(_row + 3, 2, new string(' ', _width));
		}

		private void Muck()
		{
			DrawMuckedSingleCard(_row + 1, 10, firstCard);
			DrawMuckedSingleCard(_row + 1, 14, secondCard);
		}

		public void DrawGameBox()
		{
			string top;
			if (Name != null && Name != string.Empty)
			{
				top = new string('═', 5) + Name + "═";
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
		}


		private void DrawMuckedSingleCard(int row, int col, Card card)
		{
			ConsoleHelper.WriteOnConsole(row, col, " " + card + " ", ConsoleColor.Gray, ConsoleColor.DarkGray);
		}

		private ConsoleColor GetCardColor(Card card)
		{
			switch (card.Suit)
			{
			case CardSuit.Club: return ConsoleColor.DarkGreen;
			case CardSuit.Diamond: return ConsoleColor.Blue;
			case CardSuit.Heart: return ConsoleColor.Red;
			case CardSuit.Spade: return ConsoleColor.Black;
			default: throw new ArgumentException("card.Suit");
			}
		}

		public override void EndHand(IEndHandContext context)
		{
			ResetWinner();
			IsDealer = false;
			base.EndHand(context);
		}
	}
}
