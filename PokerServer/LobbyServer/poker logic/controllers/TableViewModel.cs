using System;
using System.Collections.Generic;
using System.Linq;
using LobbyServer.pokerlogic.Cards;
using LobbyServer.pokerlogic.Extensions;
using LobbyServer.pokerlogic.pokermodel.Players;
using LobbyServer.pokerlogic.pokermodel.UI;

namespace LobbyServer.pokerlogic.controllers
{
	/*

		StaticLogger.Print($"Lobby_Client_Session + {Id.ToString().Split('-')[0]}", "disconnect all users");
		StaticLogger.Print($"Lobby_Client_Session + {Id.ToString().Split('-')[0]}",
			new string[]
			{
				"multicasting for all users",
				text
			}
		);

	 */
	public class TableViewModel
	{
		private IReadOnlyCollection<Card> CommunityCards { get; set; }
		private readonly int _commonRow;
		private readonly int _width;

		public TableViewModel(int commonRow, int width)
		{
			_width = width;
			_commonRow = commonRow;
		}

		public void StartHand()
		{
			UpdateCommonRows(0, 0, new int[] { });
		}

		public void StartRound(IReadOnlyCollection<Card> communityCards, int currentPot, int mainPot, IEnumerable<int> sidePots)
		{
			CommunityCards = communityCards;
			DrawCommunityCards();
			UpdateCommonRows(currentPot, mainPot, sidePots);
		}

		public void UpdateTableBeforeTurn(IGetTurnContext context)
		{
			UpdateCommonRows(
				context.CurrentPot,
				context.MainPot.AmountOfMoney,
				context.SidePots.Select(s => s.AmountOfMoney));
		}

		public void UpdateTableBeforePostingBlind(IPostingBlindContext context)
		{
			UpdateCommonRows(context.CurrentPot, context.CurrentPot, new int[] { });
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

		private void DrawCommunityCards()
		{
			if (CommunityCards != null)
			{
				var cardsStartCol = 0;
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

		public void ClearCards()
		{
			CommunityCards = null;

			ConsoleHelper.WriteOnConsole(_commonRow, 0, "                       ", ConsoleColor.Black, ConsoleColor.Black);
		}

		private void DrawSingleCard(int row, int col, Card card)
		{
			var cardColor = GetCardColor(card);
			ConsoleHelper.WriteOnConsole(row, col, " " + card + " ", cardColor, ConsoleColor.White);
			ConsoleHelper.WriteOnConsole(row, col + 2 + card.ToString().Length, " ");
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
	}
}
