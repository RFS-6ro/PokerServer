using GameCore.Card.Poker;

namespace TexasHoldem.Logic.Cards
{
    public interface IDeck
    {
        CardData GetNextCard();
    }
}
