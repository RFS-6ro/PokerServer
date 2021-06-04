﻿namespace TexasHoldem.Logic.Players
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using TexasHoldem.Logic.Cards;
	using TexasHoldem.Logic.GameMechanics;

	public abstract class PlayerDecorator : IPlayer
	{
		protected PlayerDecorator() { }

		public virtual string Name => Player.Name;

		public int BuyIn => Player.BuyIn;

		public List<Card> Cards { get; protected set; } = new List<Card>();

		public InternalPlayerMoney PlayerMoney { get; protected set; }

		protected IPlayer Player { get; set; }

		public virtual void SetPlayer(IPlayer player)
		{
			Player = player;
		}

		public virtual void StartGame(IStartGameContext context)
		{
			PlayerMoney = new InternalPlayerMoney(context.StartMoney);
			Player.StartGame(context);
		}

		public virtual void StartHand(IStartHandContext context)
		{
			Cards.Clear();

			PlayerMoney.NewHand();

			Player.StartHand(context);
		}

		public virtual void AddCard(Card card, bool isFirstCard)
		{
			Cards.Add(card);
			Player.AddCard(card, isFirstCard);
		}

		public virtual void StartRound(IStartRoundContext context)
		{
			PlayerMoney.NewRound(context.RoundType);
			Player.StartRound(context);
		}

		public virtual PlayerAction PostingBlind(IPostingBlindContext context)
		{
			return Player.PostingBlind(context);
		}

		public virtual PlayerAction GetTurn(IGetTurnContext context)
		{
			return Player.GetTurn(context);
		}

		public virtual void EndRound(IEndRoundContext context)
		{
			Player.EndRound(context);
		}

		public virtual void EndHand(IEndHandContext context)
		{
			Player.EndHand(context);
		}

		public virtual void EndGame(IEndGameContext context)
		{
			Player.EndGame(context);
		}

		public virtual async Task AwaitTurn(Action<PlayerAction> action, IGetTurnContext context)
		{
			await Player.AwaitTurn(action, context);
		}
	}
}
