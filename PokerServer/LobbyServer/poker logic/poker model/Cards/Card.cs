namespace LobbyServer.pokerlogic.Cards
{
	/// <summary>
	/// Immutable object to represent game card with suit and type.
	/// </summary>
	public class Card : IDeepCloneable<Card>
	{
		public Card(CardSuit suit, CardType type)
		{
			Suit = suit;
			Type = type;
		}

		public CardSuit Suit { get; }

		public CardType Type { get; }

		public static Card FromHashCode(int hashCode)
		{
			var suitId = hashCode / 13;
			return new Card((CardSuit)suitId, (CardType)(hashCode - (suitId * 13) + 2));
		}

		public override bool Equals(object obj)
		{
			var anotherCard = obj as Card;
			return anotherCard != null && Equals(anotherCard);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((int)Suit * 13) + (int)Type - 2;
			}
		}

		public Card DeepClone()
		{
			return new Card(Suit, Type);
		}

		public override string ToString()
		{
			return $"{Type.ToFriendlyString()}{Suit.ToFriendlyString()}";
		}

		private bool Equals(Card other)
		{
			return Suit == other.Suit && Type == other.Type;
		}
	}
}
