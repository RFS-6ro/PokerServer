namespace TexasHoldem.Logic.Players
{
    using GameCore.Card.Poker;
    using System.Collections.Generic;

    public class EndHandContext : IEndHandContext
    {
        public EndHandContext(Dictionary<string, ICollection<CardData>> showdownCards)
        {
            this.ShowdownCards = showdownCards;
        }

        public Dictionary<string, ICollection<CardData>> ShowdownCards { get; private set; }
    }
}
