using PokerSynchronisation;
using System.Collections.Generic;
using LobbyServer.pokerlogic.Cards;
using LobbyServer.pokerlogic.GameMechanics;

namespace LobbyServer.pokerlogic.Contexts
{
	public class StartRoundContext
	{
		public GameRoundType RoundType { get; }
		public IReadOnlyCollection<Card> CommunityCards { get; }
		public int MoneyLeft { get; }
		public int CurrentPot { get; }
		public Pot CurrentMainPot { get; }
		public IReadOnlyCollection<Pot> CurrentSidePots { get; }

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
	}
}
