namespace LobbyServer.pokerlogic.pokermodel.Players
{
	using System.Collections.Generic;

	public class EndRoundContext : IEndRoundContext
	{
		public EndRoundContext(IReadOnlyCollection<PlayerActionAndName> roundActions)
		{
			RoundActions = roundActions;
		}

		public IReadOnlyCollection<PlayerActionAndName> RoundActions { get; }
	}
}
