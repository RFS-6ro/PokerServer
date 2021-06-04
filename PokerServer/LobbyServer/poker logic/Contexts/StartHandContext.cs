using TexasHoldem.Logic.Cards;

namespace GameCore.Poker.Contexts
{
	public class StartHandContext
	{
		public Card FirstCard { get; }
		public Card SecondCard { get; }
		public int HandNumber { get; }
		public int MoneyLeft { get; }
		public int SmallBlind { get; }
		public string FirstPlayerName { get; }

		public StartHandContext(Card firstCard, Card secondCard, int handNumber, int moneyLeft, int smallBlind, string firstPlayerName)
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
