namespace TexasHoldem.Logic.Players
{
    using GameCore.Card.Poker;
    using System.Collections.Generic;

    public interface IEndHandContext
    {
        Dictionary<string, ICollection<CardData>> ShowdownCards { get; }
    }
}
