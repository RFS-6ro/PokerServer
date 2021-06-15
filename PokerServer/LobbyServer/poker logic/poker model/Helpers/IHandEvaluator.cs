namespace LobbyServer.pokerlogic.Helpers
{
	using System.Collections.Generic;
	using LobbyServer.pokerlogic.Cards;

	public interface IHandEvaluator
	{
		BestHand GetBestHand(IEnumerable<Card> cards);
	}
}
