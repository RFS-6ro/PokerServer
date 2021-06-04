namespace TexasHoldem.Logic.Helpers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using TexasHoldem.Logic.Cards;

	public class BestHand : IComparable<BestHand>
	{
		public BestHand(HandRankType rankType, ICollection<int> cards)
		{
			if (cards.Count != 5)
			{
				throw new ArgumentException("Cards collection should contains exactly 5 elements", nameof(cards));
			}

			Cards = cards;
			RankType = rankType;
		}

		// When comparing or ranking cards, the suit doesn't matter
		public ICollection<int> Cards { get; }

		public HandRankType RankType { get; }

		public int CompareTo(BestHand other)
		{
			if (RankType > other.RankType)
			{
				return 1;
			}

			if (RankType < other.RankType)
			{
				return -1;
			}

			switch (RankType)
			{
			case HandRankType.HighCard:
				return CompareTwoHandsWithHighCard(Cards, other.Cards);
			case HandRankType.Pair:
				return CompareTwoHandsWithPair(Cards, other.Cards);
			case HandRankType.TwoPairs:
				return CompareTwoHandsWithTwoPairs(Cards, other.Cards);
			case HandRankType.ThreeOfAKind:
				return CompareTwoHandsWithThreeOfAKind(Cards, other.Cards);
			case HandRankType.Straight:
				return CompareTwoHandsWithStraight(Cards, other.Cards);
			case HandRankType.Flush:
				return CompareTwoHandsWithHighCard(Cards, other.Cards);
			case HandRankType.FullHouse:
				return CompareTwoHandsWithFullHouse(Cards, other.Cards);
			case HandRankType.FourOfAKind:
				return CompareTwoHandsWithFourOfAKind(Cards, other.Cards);
			case HandRankType.StraightFlush:
				return CompareTwoHandsWithStraight(Cards, other.Cards);
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		private static int CompareTwoHandsWithHighCard(
			ICollection<int> firstHand,
			ICollection<int> secondHand)
		{
			var firstSorted = firstHand.OrderByDescending(x => x).ToList();
			var secondSorted = secondHand.OrderByDescending(x => x).ToList();
			var cardsToCompare = Math.Min(firstHand.Count, secondHand.Count);
			for (var i = 0; i < cardsToCompare; i++)
			{
				if (firstSorted[i] > secondSorted[i])
				{
					return 1;
				}

				if (firstSorted[i] < secondSorted[i])
				{
					return -1;
				}
			}

			return 0;
		}

		private static int CompareTwoHandsWithPair(
			ICollection<int> firstHand,
			ICollection<int> secondHand)
		{
			var firstPairType = firstHand.GroupBy(x => x).First(x => x.Count() >= 2);
			var secondPairType = secondHand.GroupBy(x => x).First(x => x.Count() >= 2);

			if (firstPairType.Key > secondPairType.Key)
			{
				return 1;
			}

			if (firstPairType.Key < secondPairType.Key)
			{
				return -1;
			}

			// Equal pair => compare high card
			return CompareTwoHandsWithHighCard(firstHand, secondHand);
		}

		private static int CompareTwoHandsWithTwoPairs(
			ICollection<int> firstHand,
			ICollection<int> secondHand)
		{
			var firstPairType = firstHand.GroupBy(x => x).Where(x => x.Count() == 2).OrderByDescending(x => x.Key).ToList();
			var secondPairType = secondHand.GroupBy(x => x).Where(x => x.Count() == 2).OrderByDescending(x => x.Key).ToList();

			for (int i = 0; i < firstPairType.Count; i++)
			{
				if (firstPairType[i].Key > secondPairType[i].Key)
				{
					return 1;
				}

				if (secondPairType[i].Key > firstPairType[i].Key)
				{
					return -1;
				}
			}

			// Equal pairs => compare high card
			return CompareTwoHandsWithHighCard(firstHand, secondHand);
		}

		private static int CompareTwoHandsWithThreeOfAKind(
			ICollection<int> firstHand,
			ICollection<int> secondHand)
		{
			var firstThreeOfAKindType = firstHand.GroupBy(x => x).Where(x => x.Count() == 3).OrderByDescending(x => x.Key).FirstOrDefault();
			var secondThreeOfAKindType = secondHand.GroupBy(x => x).Where(x => x.Count() == 3).OrderByDescending(x => x.Key).FirstOrDefault();
			if (firstThreeOfAKindType.Key > secondThreeOfAKindType.Key)
			{
				return 1;
			}

			if (secondThreeOfAKindType.Key > firstThreeOfAKindType.Key)
			{
				return -1;
			}

			// Equal triples => compare high card
			return CompareTwoHandsWithHighCard(firstHand, secondHand);
		}

		private static int CompareTwoHandsWithStraight(
			ICollection<int> firstHand,
			ICollection<int> secondHand)
		{
			var firstBiggestCardType = firstHand.Max();
			if (firstBiggestCardType == (int)CardType.Ace && firstHand.Contains((int)CardType.Five))
			{
				firstBiggestCardType = (int)CardType.Five;
			}

			var secondBiggestCardType = secondHand.Max();
			if (secondBiggestCardType == (int)CardType.Ace && secondHand.Contains((int)CardType.Five))
			{
				secondBiggestCardType = (int)CardType.Five;
			}

			return firstBiggestCardType.CompareTo(secondBiggestCardType);
		}

		private static int CompareTwoHandsWithFullHouse(
			ICollection<int> firstHand,
			ICollection<int> secondHand)
		{
			var firstThreeOfAKindType = firstHand.GroupBy(x => x).Where(x => x.Count() == 3).OrderByDescending(x => x.Key).FirstOrDefault();
			var secondThreeOfAKindType = secondHand.GroupBy(x => x).Where(x => x.Count() == 3).OrderByDescending(x => x.Key).FirstOrDefault();

			if (firstThreeOfAKindType.Key > secondThreeOfAKindType.Key)
			{
				return 1;
			}

			if (secondThreeOfAKindType.Key > firstThreeOfAKindType.Key)
			{
				return -1;
			}

			var firstPairType = firstHand.GroupBy(x => x).First(x => x.Count() == 2);
			var secondPairType = secondHand.GroupBy(x => x).First(x => x.Count() == 2);
			return firstPairType.Key.CompareTo(secondPairType.Key);
		}

		private static int CompareTwoHandsWithFourOfAKind(
			ICollection<int> firstHand,
			ICollection<int> secondHand)
		{
			var firstFourOfAKingType = firstHand.GroupBy(x => x).First(x => x.Count() == 4);
			var secondFourOfAKindType = secondHand.GroupBy(x => x).First(x => x.Count() == 4);

			if (firstFourOfAKingType.Key > secondFourOfAKindType.Key)
			{
				return 1;
			}

			if (firstFourOfAKingType.Key < secondFourOfAKindType.Key)
			{
				return -1;
			}

			// Equal pair => compare high card
			return CompareTwoHandsWithHighCard(firstHand, secondHand);
		}
	}
}
