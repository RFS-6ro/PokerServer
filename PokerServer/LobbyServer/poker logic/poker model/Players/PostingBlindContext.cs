namespace LobbyServer.pokerlogic.pokermodel.Players
{
	public class PostingBlindContext : IPostingBlindContext
	{
		public PostingBlindContext(PlayerAction blindAction, int currentPot, int currentStackSize)
		{
			BlindAction = blindAction;
			CurrentPot = currentPot;
			CurrentStackSize = currentStackSize;
		}

		public PlayerAction BlindAction { get; }

		public int CurrentPot { get; }

		public int CurrentStackSize { get; }
	}
}
