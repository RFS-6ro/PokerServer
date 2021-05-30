namespace TexasHoldem.Logic.Players
{
    using GameCore.Card.Poker;
    using PokerSynchronisation;
    using System.Collections.Generic;
    using TexasHoldem.Logic.GameMechanics;


    public interface IStartRoundContext
    {
        IReadOnlyCollection<CardData> CommunityCards { get; }

        int CurrentPot { get; }

        int MoneyLeft { get; }

        GameRoundType RoundType { get; }

        Pot CurrentMainPot { get; }

        IReadOnlyCollection<Pot> CurrentSidePots { get; }
    }
}
