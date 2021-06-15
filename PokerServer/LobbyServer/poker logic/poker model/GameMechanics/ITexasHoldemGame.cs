namespace LobbyServer.pokerlogic.GameMechanics
{
	using LobbyServer.pokerlogic.pokermodel.Players;

	public interface ITexasHoldemGame
	{
		int HandsPlayed { get; }

		IPlayer Start();
	}
}
