namespace LobbyServer.pokerlogic.Contexts
{
	public class EndGameContext
	{
		public string WinnerName { get; }

		public EndGameContext(string winnerName)
		{
			WinnerName = winnerName;
		}
	}
}
