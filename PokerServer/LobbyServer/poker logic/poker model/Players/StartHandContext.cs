namespace LobbyServer.pokerlogic.pokermodel.Players
{
	public class StartHandContext : IStartHandContext
	{
		public StartHandContext(int handNumber,
								int moneyLeft,
								int smallBlind,
								string firstPlayerName)
		{
			HandNumber = handNumber;
			MoneyLeft = moneyLeft;
			SmallBlind = smallBlind;
			FirstPlayerName = firstPlayerName;
		}

		public int HandNumber { get; }

		public int MoneyLeft { get; }

		public int SmallBlind { get; }

		public string FirstPlayerName { get; }
	}
}
