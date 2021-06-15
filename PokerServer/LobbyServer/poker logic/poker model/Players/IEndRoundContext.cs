namespace LobbyServer.pokerlogic.pokermodel.Players
{
	using System.Collections.Generic;

	public interface IEndRoundContext
	{
		IReadOnlyCollection<PlayerActionAndName> RoundActions { get; }
	}
}
