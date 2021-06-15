namespace LobbyServer.pokerlogic.pokermodel.Players
{
	using PokerSynchronisation;
	using System.Collections.Generic;
	using LobbyServer.pokerlogic.Cards;
	using LobbyServer.pokerlogic.GameMechanics;


	public interface IStartRoundContext
	{
		IReadOnlyCollection<Card> CommunityCards { get; }

		int CurrentPot { get; }

		int MoneyLeft { get; }

		GameRoundType RoundType { get; }

		Pot CurrentMainPot { get; }

		IReadOnlyCollection<Pot> CurrentSidePots { get; }
	}
}
