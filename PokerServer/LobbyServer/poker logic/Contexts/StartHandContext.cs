using GameCore.Card.Poker;
using GameCore.SaveSystem;

namespace GameCore.Poker.Contexts
{
    public class StartHandContext
    {
        public CardModel FirstCard { get; }
        public CardModel SecondCard { get; }
        public SafeInt HandNumber { get; }
        public SafeInt MoneyLeft { get; }
        public SafeInt SmallBlind { get; }
        public string FirstPlayerName { get; }

        public StartHandContext(CardModel firstCard, CardModel secondCard, SafeInt handNumber, SafeInt moneyLeft, SafeInt smallBlind, string firstPlayerName)
        {
            FirstCard = firstCard;
            SecondCard = secondCard;
            HandNumber = handNumber;
            MoneyLeft = moneyLeft;
            SmallBlind = smallBlind;
            FirstPlayerName = firstPlayerName;
        }
    }
}
