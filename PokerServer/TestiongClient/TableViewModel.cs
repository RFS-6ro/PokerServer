﻿using System;
using System.Collections.Generic;
using System.Data;

namespace TestingClient
{
	public class TableViewModel
	{
		private int _width;
		private List<(int, int)> CommunityCards = new();

		public TableViewModel(int width)
		{
			_width = width;
		}

		public void ShowPot(int amount)
		{
			// Clear the first common row
			ConsoleHelper.WriteOnConsole(1, 0, new string(' ', _width - 1));

			DrawCommunityCards();

			var potAsString = "Pot: " + amount;
			ConsoleHelper.WriteOnConsole(1, _width - potAsString.Length - 2, potAsString);
		}

		public void AddCards(List<(int, int)> cards)
		{
			CommunityCards.AddRange(cards);
			DrawCommunityCards();
		}

		public void DrawCommunityCards()
		{
			int cardsStartCol;
			if (CommunityCards.Count < 0)
			{
				cardsStartCol = (_width / 2) - ((17 / 2) / 2);
				ConsoleHelper.WriteOnConsole(1, cardsStartCol, "                 ");
				return;
			}

			string cardsAsString = CommunityCards.CardsToString();
			int cardsLength = cardsAsString.Length / 2;
			cardsStartCol = (_width / 2) - (cardsLength / 2);
			int cardIndex = 0;
			int spacing = 0;

			foreach (var communityCard in CommunityCards)
			{
				DrawSingleCard(1, cardsStartCol + (cardIndex * 4) + spacing, communityCard.Item1, communityCard.Item2);
				cardIndex++;

				spacing += communityCard.Item1 == 10 ? 1 : 0;
			}
		}

		private void DrawSingleCard(int row, int col, int type, int suit)
		{
			var cardColor = ConsoleHelper.GetCardColor(type, suit);
			ConsoleHelper.WriteOnConsole(row, col, " " + ConsoleHelper.ToFriendlyString(type, suit) + " ", cardColor, ConsoleColor.White);
			ConsoleHelper.WriteOnConsole(row, col + 2 + ConsoleHelper.ToFriendlyString(type, suit).Length, " ");
		}

		public void ClearCards()
		{
			CommunityCards.Clear();
			DrawCommunityCards();
		}
	}
}
