namespace TexasHoldem.Logic.Players
{
	using PokerSynchronisation;
	using System.Collections.Generic;
	using TexasHoldem.Logic.Cards;
	using TexasHoldem.Logic.GameMechanics;

	public class StartRoundContext : IStartRoundContext
	{
		public StartRoundContext(
			GameRoundType roundType,
			IReadOnlyCollection<Card> communityCards,
			int moneyLeft,
			int currentPot,
			Pot currentMainPot,
			List<Pot> currentSidePots)
		{
			RoundType = roundType;
			CommunityCards = communityCards;
			MoneyLeft = moneyLeft;
			CurrentPot = currentPot;
			CurrentMainPot = currentMainPot;
			CurrentSidePots = currentSidePots;
		}

		public GameRoundType RoundType { get; }

		public IReadOnlyCollection<Card> CommunityCards { get; }

		public int MoneyLeft { get; }

		public int CurrentPot { get; }

		public Pot CurrentMainPot { get; }

		public IReadOnlyCollection<Pot> CurrentSidePots { get; }
	}
}
