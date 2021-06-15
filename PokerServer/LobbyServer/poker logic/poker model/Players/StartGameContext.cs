namespace LobbyServer.pokerlogic.pokermodel.Players
{
	using System.Collections.Generic;

	public class StartGameContext : IStartGameContext
	{
		public StartGameContext(IReadOnlyCollection<string> playerNames, int startMoney)
		{
			PlayerNames = playerNames;
			StartMoney = startMoney;
		}

		public IReadOnlyCollection<string> PlayerNames { get; }

		public int StartMoney { get; }
	}
}
