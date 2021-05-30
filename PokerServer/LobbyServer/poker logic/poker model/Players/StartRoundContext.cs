namespace TexasHoldem.Logic.Players
{
    using GameCore.Card.Poker;
    using PokerSynchronisation;
    using System.Collections.Generic;
    using TexasHoldem.Logic.GameMechanics;

    public class StartRoundContext : IStartRoundContext
    {
        public StartRoundContext(
            GameRoundType roundType,
            IReadOnlyCollection<CardData> communityCards,
            int moneyLeft,
            int currentPot,
            Pot currentMainPot,
            List<Pot> currentSidePots)
        {
            this.RoundType = roundType;
            this.CommunityCards = communityCards;
            this.MoneyLeft = moneyLeft;
            this.CurrentPot = currentPot;
            this.CurrentMainPot = currentMainPot;
            this.CurrentSidePots = currentSidePots;
        }

        public GameRoundType RoundType { get; }

        public IReadOnlyCollection<CardData> CommunityCards { get; }

        public int MoneyLeft { get; }

        public int CurrentPot { get; }

        public Pot CurrentMainPot { get; }

        public IReadOnlyCollection<Pot> CurrentSidePots { get; }
    }
}
