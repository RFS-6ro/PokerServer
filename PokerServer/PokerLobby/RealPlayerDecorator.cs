using System.Collections.Generic;
using TexasHoldem.Logic.Cards;
using TexasHoldem.Logic.Players;

namespace PokerLobby
{
	public class RealPlayerDecorator : PlayerDecorator
	{
		public RealPlayerDecorator() : base()
		{
			Cards = new List<Card>();
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
			PlayerMoney = new TexasHoldem.Logic.GameMechanics.InternalPlayerMoney(context.StartMoney);
			base.StartGame(context);
		}

		public override void StartHand(IStartHandContext context)
		{
			Cards.Clear();
			Cards.Add(context.FirstCard);
			Cards.Add(context.SecondCard);

			PlayerMoney.NewHand();
			base.StartHand(context);
		}

		public override void StartRound(IStartRoundContext context)
		{
			PlayerMoney.NewRound();
			base.StartRound(context);
		}
	}
}
