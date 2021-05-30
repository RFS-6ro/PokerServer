namespace TexasHoldem.Logic.Players
{
	using GameCore.Card.Poker;
	using System;
	using System.Collections;
	using System.Collections.Generic;

	public abstract class BasePlayer : IPlayer
	{
		public abstract string Name { get; }

		public abstract int BuyIn { get; }

		protected IReadOnlyCollection<CardData> CommunityCards { get; private set; }

		protected CardData FirstCard { get; private set; }

		protected CardData SecondCard { get; private set; }

		public virtual void StartGame(IStartGameContext context) { }

		public virtual void StartHand(IStartHandContext context) { }

		public Action<CardModel> AddCard(CardData card, bool isFirstCard)
		{
			if (isFirstCard)
			{
				FirstCard = card;
			}
			else
			{
				SecondCard = card;
			}

			return null;
		}

		public virtual void StartRound(IStartRoundContext context)
		{
			CommunityCards = context.CommunityCards;
		}

		public abstract PlayerAction PostingBlind(IPostingBlindContext context);

		public abstract PlayerAction GetTurn(IGetTurnContext context);

		public virtual void EndRound(IEndRoundContext context) { }

		public virtual void EndHand(IEndHandContext context) { }

		public virtual void EndGame(IEndGameContext context) { }

		public virtual IEnumerator AwaitTurn(Action<PlayerAction> action, IGetTurnContext context) { throw new NotImplementedException(); }
	}
}
