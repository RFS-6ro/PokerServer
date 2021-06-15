namespace LobbyServer.pokerlogic.pokermodel.Players
{
	public interface IPostingBlindContext
	{
		PlayerAction BlindAction { get; }

		int CurrentStackSize { get; }

		int CurrentPot { get; }
	}
}
