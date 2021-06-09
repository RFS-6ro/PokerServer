namespace TexasHoldem.Logic.GameMechanics
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using TexasHoldem.Logic.Cards;
	using TexasHoldem.Logic.Players;

	public class InternalPlayer : PlayerDecorator
	{
		public InternalPlayer(IPlayer player)
			: base()
		{
			Player = player;
			Cards = new List<Card>();
		}

		public override async Task<PlayerAction> AwaitTurn(IGetTurnContext context)
		{
			throw new NotImplementedException();
		}

		public override void StartGame(IStartGameContext context)
		{
			PlayerMoney = new InternalPlayerMoney(context.StartMoney);
			base.StartGame(context);
		}

		public override void StartHand(IStartHandContext context)
		{

			base.StartHand(context);
		}

		public override void StartRound(IStartRoundContext context)
		{
			PlayerMoney.NewRound(context.RoundType);
			base.StartRound(context);
		}
	}
}
