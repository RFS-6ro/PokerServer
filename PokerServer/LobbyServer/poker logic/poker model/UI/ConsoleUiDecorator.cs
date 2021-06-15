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

		private const ConsoleColor PlayerBoxColor = ConsoleColor.DarkGreen;

		private int _row;

		private int _width;

		private int _commonRow;

		private Card firstCard;

		private Card secondCard;

		private IReadOnlyCollection<Card> CommunityCards { get; set; }

		private Lobby_Client_Server Server => IStaticInstance<Lobby_Client_Server>.Instance;
		private SessionSender<Lobby_Client_Server> Sender => IStaticInstance<Lobby_Client_Server>.Instance.SendHandler;

		public bool IsDealer;

		public override void SetPlayer(IPlayer player)
		{
			base.SetPlayer(player);

			if (player.GetType() == typeof(ServerPlayer))
			{
				PlayerGuid = ((ServerPlayer)player).Guid;
			}
		}

		public override void StartHand(IStartHandContext context)
		{
			UpdateCommonRows(0, 0, new int[] { });
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
			CommunityCards = context.CommunityCards;
			UpdateCommonRows(
				context.CurrentPot,
				context.CurrentMainPot.AmountOfMoney,
				context.CurrentSidePots.Select(s => s.AmountOfMoney));

			ConsoleHelper.WriteOnConsole(_row + 1, _width - 11, context.RoundType + "   ");
			ConsoleHelper.WriteOnConsole(_row + 3, 2, new string(' ', _width - 3));
			base.StartRound(context);
		}

		public override PlayerAction PostingBlind(IPostingBlindContext context)
		{
			UpdateCommonRows(context.CurrentPot, context.CurrentPot, new int[] { });

			var action = base.PostingBlind(context);

			ConsoleHelper.WriteOnConsole(_row + 2, 2, new string(' ', _width - 3));
			ConsoleHelper.WriteOnConsole(_row + 3, 2, "Last action: " + action.Type + "(" + action.Money + ")");

			var moneyAfterAction = context.CurrentStackSize;

			ConsoleHelper.WriteOnConsole(_row + 1, 2, moneyAfterAction + "   ");

			return action;
		}

		public async override Task<PlayerAction> GetTurn(IGetTurnContext context)
		{
			UpdateCommonRows(
				context.CurrentPot,
				context.MainPot.AmountOfMoney,
				context.SidePots.Select(s => s.AmountOfMoney));

			ConsoleHelper.WriteOnConsole(_row + 1, 2, context.MoneyLeft + "   ");

			var action = await base.GetTurn(context);

			if (action.Type == TurnType.Fold)
			{
				Muck(context.MoneyLeft);
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

		public async override Task<PlayerAction> AwaitTurn(IGetTurnContext context)
		{
			PlayerAction action = await GetTurn(context);

			return action;
		}

		private void Muck(int moneyLeft)
		{
			DrawMuckedSingleCard(_row + 1, 10, firstCard);
			DrawMuckedSingleCard(_row + 1, 14, secondCard);
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

			if (Name != null && Name != string.Empty)
			{
				string top = new string('═', 5) + Name + new string('═', 5);
				ConsoleHelper.WriteOnConsole(_row, 0, top, PlayerBoxColor);
			}
			else
			{
				ConsoleHelper.WriteOnConsole(_row, 0, new string('═', _width), PlayerBoxColor);
			}
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

		public override void EndHand(IEndHandContext context)
		{
			IsDealer = false;
			base.EndHand(context);
		}
	}
}
