using GameCore.Card.Poker;
using System.Collections.Generic;

namespace GameCore.Poker.Contexts
{
    public class EndHandContext
    {
        public Dictionary<string, ICollection<CardModel>> ShowdownCards { get; private set; }

        public EndHandContext(Dictionary<string, ICollection<CardModel>> showdownCards)
        {
            ShowdownCards = showdownCards;
        }
    }
}
