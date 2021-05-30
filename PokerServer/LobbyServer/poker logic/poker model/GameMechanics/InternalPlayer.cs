namespace TexasHoldem.Logic.GameMechanics
{
	using GameCore.Card.Poker;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using TexasHoldem.Logic.Players;

	public class InternalPlayer : PlayerDecorator
	{
		public InternalPlayer(IPlayer player)
			: base()
		{
			this.Player = player;
			this.Cards = new List<CardData>();
		}

		public override Action<CardModel> AddCard(CardData card, bool isFirstCard)
		{
			base.AddCard(card, isFirstCard);
			return null;
		}

		public override IEnumerator AwaitTurn(Action<Players.PlayerAction> action, IGetTurnContext context)
		{
			throw new NotImplementedException();
		}

		public override void StartGame(IStartGameContext context)
		{
			this.PlayerMoney = new InternalPlayerMoney(context.StartMoney);
			base.StartGame(context);
		}

		public override void StartHand(IStartHandContext context)
		{

			base.StartHand(context);
		}

		public override void StartRound(IStartRoundContext context)
		{
			this.PlayerMoney.NewRound(context.RoundType);
			base.StartRound(context);
		}
	}
}
