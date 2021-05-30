using GameCore.Card.Poker;
using GameCore.SaveSystem;
using System.Collections.Generic;

namespace GameCore.Poker.Contexts
{
    public class StartRoundContext
    {
        public GameRoundType RoundType { get; }
        public IReadOnlyCollection<CardModel> CommunityCards { get; }
        public SafeInt MoneyLeft { get; }
        public SafeInt CurrentPot { get; }
        public Pot CurrentMainPot { get; }
        public IReadOnlyCollection<Pot> CurrentSidePots { get; }

        public StartRoundContext(
            GameRoundType roundType,
            IReadOnlyCollection<CardModel> communityCards,
            SafeInt moneyLeft,
            SafeInt currentPot,
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
