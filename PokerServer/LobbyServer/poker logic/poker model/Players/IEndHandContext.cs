namespace LobbyServer.pokerlogic.pokermodel.Players
{
	using System.Collections.Generic;
	using LobbyServer.pokerlogic.Cards;

	public interface IEndHandContext
	{
		Dictionary<string, ICollection<Card>> ShowdownCards { get; }
	}
}
