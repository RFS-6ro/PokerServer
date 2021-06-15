namespace LobbyServer.pokerlogic.pokermodel.Players
{
	public interface IStartHandContext
	{
		string FirstPlayerName { get; }

		int HandNumber { get; }

		int MoneyLeft { get; }

		int SmallBlind { get; }
	}
}
