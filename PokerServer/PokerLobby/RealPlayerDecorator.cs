using TexasHoldem.Logic.Players;

namespace PokerLobby
{
	public class RealPlayerDecorator : PlayerDecorator
	{
		public RealPlayerDecorator(IPlayer player) : base(player)
		{
		}

		public override void EndGame(IEndGameContext context)
		{
			base.EndGame(context);
		}

		public override void EndHand(IEndHandContext context)
		{
			base.EndHand(context);
		}

		public override void EndRound(IEndRoundContext context)
		{
			base.EndRound(context);
		}

		public override PlayerAction GetTurn(IGetTurnContext context)
		{
			return base.GetTurn(context);
		}

		public override PlayerAction PostingBlind(IPostingBlindContext context)
		{
			return base.PostingBlind(context);
		}

		public override void StartGame(IStartGameContext context)
		{
			base.StartGame(context);
		}

		public override void StartHand(IStartHandContext context)
		{
			base.StartHand(context);
		}

		public override void StartRound(IStartRoundContext context)
		{
			base.StartRound(context);
		}
	}
}
