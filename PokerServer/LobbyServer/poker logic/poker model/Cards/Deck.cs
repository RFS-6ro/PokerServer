namespace TexasHoldem.Logic.Cards
{
	using GameCore.Card.Poker;
	using System.Collections.Generic;
	using System.Linq;
	using TexasHoldem.Logic.Extensions;

	public class Deck : IDeck
	{
		public static readonly IReadOnlyList<CardData> AllCards;

		private static readonly IEnumerable<GameCore.Card.Poker.CardType> AllCardTypes = new List<GameCore.Card.Poker.CardType>
																		 {
																			 GameCore.Card.Poker.CardType.Two,
																			 GameCore.Card.Poker.CardType.Three,
																			 GameCore.Card.Poker.CardType.Four,
																			 GameCore.Card.Poker.CardType.Five,
																			 GameCore.Card.Poker.CardType.Six,
																			 GameCore.Card.Poker.CardType.Seven,
																			 GameCore.Card.Poker.CardType.Eight,
																			 GameCore.Card.Poker.CardType.Nine,
																			 GameCore.Card.Poker.CardType.Ten,
																			 GameCore.Card.Poker.CardType.Jack,
																			 GameCore.Card.Poker.CardType.Queen,
																			 GameCore.Card.Poker.CardType.King,
																			 GameCore.Card.Poker.CardType.Ace,
																		 };

		private static readonly IEnumerable<GameCore.Card.Poker.CardSuit> AllCardSuits = new List<GameCore.Card.Poker.CardSuit>
																		 {
																			 GameCore.Card.Poker.CardSuit.Club,
																			 GameCore.Card.Poker.CardSuit.Diamond,
																			 GameCore.Card.Poker.CardSuit.Heart,
																			 GameCore.Card.Poker.CardSuit.Spade,
																		 };

		private readonly IList<CardData> listOfCards;

		private int cardIndex;

		static Deck()
		{
			var cards = new List<CardData>();
			foreach (var cardSuit in AllCardSuits)
			{
				foreach (var cardType in AllCardTypes)
				{
					cards.Add(new CardData((int)cardSuit, (int)cardType));
				}
			}

			AllCards = cards.AsReadOnly();
		}

		public Deck()
		{
			listOfCards = AllCards.Shuffle().ToList();
			cardIndex = AllCards.Count;
		}

		public CardData GetNextCard()
		{
			if (cardIndex == 0)
			{
				throw new InternalGameException("Deck is empty!");
			}

			cardIndex--;
			var card = listOfCards[cardIndex];
			return card;
		}
	}
}
