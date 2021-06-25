namespace LobbyServer.pokerlogic.pokermodel.Players
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using LobbyServer.pokerlogic.Cards;

	public abstract class BasePlayer : IPlayer
	{
		public abstract string Name { get; }

		public abstract int BuyIn { get; }

		protected IReadOnlyCollection<Card> CommunityCards { get; private set; }

		protected Card FirstCard { get; private set; }

		protected Card SecondCard { get; private set; }

		public virtual void StartGame(IStartGameContext context) { }

		public virtual void StartHand(IStartHandContext context) { }

		public void AddCard(Card card, bool isFirstCard)
		{
			if (isFirstCard)
			{
				FirstCard = card;
			}
			else
			{
				SecondCard = card;
			}
		}

		public virtual void StartRound(IStartRoundContext context)
		{
			CommunityCards = context.CommunityCards;
		}

		public abstract PlayerAction PostingBlind(IPostingBlindContext context);

		public abstract Task<PlayerAction> GetTurn(IGetTurnContext context);

		public virtual void EndRound(IEndRoundContext context) { }

		public virtual void EndHand(IEndHandContext context) { }

		public virtual void EndGame(IEndGameContext context) { }

		public virtual Task<PlayerAction> AwaitTurn(IGetTurnContext context) { throw new NotImplementedException(); }
	}
}
