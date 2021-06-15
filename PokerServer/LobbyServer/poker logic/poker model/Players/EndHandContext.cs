namespace LobbyServer.pokerlogic.pokermodel.Players
{
	using System.Collections.Generic;
	using LobbyServer.pokerlogic.Cards;

	public class EndHandContext : IEndHandContext
	{
		public EndHandContext(Dictionary<string, ICollection<Card>> showdownCards)
		{
			ShowdownCards = showdownCards;
		}

		public Dictionary<string, ICollection<Card>> ShowdownCards { get; private set; }
	}
}
