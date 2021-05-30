namespace TexasHoldem.Logic.Helpers
{
    using GameCore.Card.Poker;
    using System.Collections.Generic;

    public interface IHandEvaluator
    {
        BestHand GetBestHand(IEnumerable<CardData> cards);
    }
}
