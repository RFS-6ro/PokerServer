namespace LobbyServer.pokerlogic.pokermodel.Players
{
	public class EndGameContext : IEndGameContext
	{
		public EndGameContext(string winnerName)
		{
			WinnerName = winnerName;
		}

		public string WinnerName { get; }
	}
}
