using System;
using System.Collections.Generic;
using System.Linq;
using PokerSynchronisation;
using TexasHoldem.Logic.Cards;
using TexasHoldem.Logic.Extensions;
using TexasHoldem.Logic.Players;
#if !DEBUG
using TexasHoldem.UI.Console;
#endif

namespace PokerLobby
{
	public class RealPlayerDecorator : PlayerDecorator
	{
		/*
		public int ServerId { get; private set; }

		public RealPlayerDecorator() : base()
		{
			Cards = new List<Card>();
		}
		public override void SetPlayer(IPlayer player)
		{
			base.SetPlayer(player);

			ServerId = ((RealPlayer)player).ServerId;
		}

		public override void EndGame(IEndGameContext context)
		{
			base.EndGame(context);
		}

		public override void EndHand(IEndHandContext context)
		{
			base.EndHand(context);
		}

		public override void EndRound(IEndRoundContext context)
		{
			base.EndRound(context);
		}

		public override PlayerAction GetTurn(IGetTurnContext context)
		{
			return base.GetTurn(context);
		}

		public override PlayerAction PostingBlind(IPostingBlindContext context)
		{
			return base.PostingBlind(context);
		}

		public override void StartGame(IStartGameContext context)
		{
			PlayerMoney = new TexasHoldem.Logic.GameMechanics.InternalPlayerMoney(context.StartMoney);
			base.StartGame(context);
		}

		public override void StartHand(IStartHandContext context)
		{
			Cards.Clear();
			Cards.Add(context.FirstCard);
			Cards.Add(context.SecondCard);

			PlayerMoney.NewHand();
			base.StartHand(context);
		}

		public override void StartRound(IStartRoundContext context)
		{
			PlayerMoney.NewRound();
			base.StartRound(context);
		}
		*/

		private const ConsoleColor _playerBoxColor = ConsoleColor.DarkGreen;

		private int _row;

		private int _width;

		private int _commonRow;

		private Card _firstCard;

		private Card _secondCard;

		private IReadOnlyCollection<Card> CommunityCards { get; set; }

		public int ServerId { get; private set; }

		public RealPlayerDecorator() : base()
		{
			Cards = new List<Card>();
		}

		public override void SetPlayer(IPlayer player)
		{
			base.SetPlayer(player);

			try
			{
				ServerId = ((RealPlayer)player).ServerId;
			}
			catch (Exception)
			{
				//bot has been connected
			}
		}

		public override void StartHand(IStartHandContext context)
		{
			Cards.Clear();
			Cards.Add(context.FirstCard.DeepClone());
			Cards.Add(context.SecondCard.DeepClone());

			PlayerMoney.NewHand();

#if !DEBUG
			UpdateCommonRows(0, 0, new int[] { });
			var dealerSymbol = context.FirstPlayerName == Player.Name ? "D" : " ";

			ConsoleHelper.WriteOnConsole(_row + 1, 1, dealerSymbol, ConsoleColor.Green);
			ConsoleHelper.WriteOnConsole(_row + 3, 2, "                            ");

			ConsoleHelper.WriteOnConsole(_row + 1, 2, context.MoneyLeft.ToString());
#endif
			_firstCard = context.FirstCard.DeepClone();
			_secondCard = context.SecondCard.DeepClone();
#if !DEBUG
			DrawSingleCard(_row + 1, 10, _firstCard);
			DrawSingleCard(_row + 1, 14, _secondCard);
#endif

			base.StartHand(context);
		}

		public override void StartRound(IStartRoundContext context)
		{
			PlayerMoney.NewRound(context.RoundType);
			CommunityCards = context.CommunityCards;
#if !DEBUG
			UpdateCommonRows(
				context.CurrentPot,
				context.CurrentMainPot.AmountOfMoney,
				context.CurrentSidePots.Select(s => s.AmountOfMoney));

			ConsoleHelper.WriteOnConsole(_row + 1, _width - 11, context.RoundType + "   ");
			ConsoleHelper.WriteOnConsole(_row + 3, 2, new string(' ', _width - 3));
#endif
			base.StartRound(context);
		}

		public override void StartGame(IStartGameContext context)
		{
			PlayerMoney = new TexasHoldem.Logic.GameMechanics.InternalPlayerMoney(context.StartMoney);
			base.StartGame(context);
		}

		public override PlayerAction PostingBlind(IPostingBlindContext context)
		{
#if !DEBUG
			UpdateCommonRows(context.CurrentPot, context.CurrentPot, new int[] { });
#endif

			var action = base.PostingBlind(context);

#if !DEBUG
			ConsoleHelper.WriteOnConsole(_row + 2, 2, new string(' ', _width - 3));
			ConsoleHelper.WriteOnConsole(_row + 3, 2, "Last action: " + action.Type + "(" + action.Money + ")");
#endif

			var moneyAfterAction = context.CurrentStackSize;

#if !DEBUG
			ConsoleHelper.WriteOnConsole(_row + 1, 2, moneyAfterAction + "   ");
#endif

			return action;
		}

		public override PlayerAction GetTurn(IGetTurnContext context)
		{
#if !DEBUG
			UpdateCommonRows(
				context.CurrentPot,
				context.MainPot.AmountOfMoney,
				context.SidePots.Select(s => s.AmountOfMoney));

			ConsoleHelper.WriteOnConsole(_row + 1, 2, context.MoneyLeft + "   ");
#endif

			var action = base.GetTurn(context);

			if (action.Type == TurnType.Fold)
			{
#if !DEBUG
				Muck(context.MoneyLeft);
#endif
			}

#if !DEBUG
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
#endif

			var moneyAfterAction = action.Type == TurnType.Fold
				? context.MoneyLeft
				: context.MoneyLeft - action.Money - context.MoneyToCall;

#if !DEBUG
			ConsoleHelper.WriteOnConsole(_row + 1, 2, moneyAfterAction + "   ");
#endif

			return action;
		}

#if !DEBUG
		private void Muck(int moneyLeft)
		{
			DrawMuckedSingleCard(_row + 1, 10, _firstCard);
			DrawMuckedSingleCard(_row + 1, 14, _secondCard);
		}
		
		private void UpdateCommonRows(int pot, int mainPot, IEnumerable<int> sidePots)
		{
			// Clear the first common row
			ConsoleHelper.WriteOnConsole(_commonRow, 0, new string(' ', _width - 1));

			DrawCommunityCards();

			var potAsString = "Pot: " + pot;
			ConsoleHelper.WriteOnConsole(_commonRow, _width - potAsString.Length - 2, potAsString);

			if (sidePots.Count() == 0)
			{
				// Clear the side pots
				ConsoleHelper.WriteOnConsole(_commonRow + 1, 0, new string(' ', _width - 1));
			}
			else
			{
				var mainPotAsString = "Main Pot: " + mainPot;
				ConsoleHelper.WriteOnConsole(_commonRow, 2, mainPotAsString);

				var sidePotsAsString = "Side Pots: ";
				foreach (var item in sidePots)
				{
					sidePotsAsString += item + " | ";
				}

				ConsoleHelper.WriteOnConsole(_commonRow + 1, 2, sidePotsAsString.Remove(sidePotsAsString.Length - 2, 2));
			}
		}

		public void DrawGameBox(int row, int width, int commonRow)
		{
			_row = row;
			_width = width;
			_commonRow = commonRow;

			ConsoleHelper.WriteOnConsole(_row, 0, new string('═', _width), _playerBoxColor);
			ConsoleHelper.WriteOnConsole(_row + 4, 0, new string('═', _width), _playerBoxColor);
			ConsoleHelper.WriteOnConsole(_row, 0, "╔", _playerBoxColor);
			ConsoleHelper.WriteOnConsole(_row, _width - 1, "╗", _playerBoxColor);
			ConsoleHelper.WriteOnConsole(_row + 4, 0, "╚", _playerBoxColor);
			ConsoleHelper.WriteOnConsole(_row + 4, _width - 1, "╝", _playerBoxColor);
			for (var i = 1; i < 4; i++)
			{
				ConsoleHelper.WriteOnConsole(_row + i, 0, "║", _playerBoxColor);
				ConsoleHelper.WriteOnConsole(_row + i, _width - 1, "║", _playerBoxColor);
			}
		}

		private void DrawCommunityCards()
		{
			if (CommunityCards != null)
			{
				var cardsAsString = CommunityCards.CardsToString();
				var cardsLength = cardsAsString.Length / 2;
				var cardsStartCol = (_width / 2) - (cardsLength / 2);
				var cardIndex = 0;
				var spacing = 0;

				foreach (var communityCard in CommunityCards)
				{
					DrawSingleCard(_commonRow, cardsStartCol + (cardIndex * 4) + spacing, communityCard);
					cardIndex++;

					spacing += communityCard.Type == CardType.Ten ? 1 : 0;
				}
			}
		}

		private void DrawSingleCard(int row, int col, Card card)
		{
			var cardColor = GetCardColor(card);
			ConsoleHelper.WriteOnConsole(row, col, " " + card + " ", cardColor, ConsoleColor.White);
			ConsoleHelper.WriteOnConsole(row, col + 2 + card.ToString().Length, " ");
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
#endif
	}
}
