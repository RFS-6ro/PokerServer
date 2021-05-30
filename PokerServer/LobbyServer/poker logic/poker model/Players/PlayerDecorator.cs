namespace TexasHoldem.Logic.Players
{
	using GameCore.Card.Poker;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using TexasHoldem.Logic.GameMechanics;
	using UnityEngine;

	public abstract class PlayerDecorator : IPlayer
	{
		protected PlayerDecorator() { }

		public virtual string Name => Player.Name;

		public int BuyIn => Player.BuyIn;

		public List<CardData> Cards { get; protected set; } = new List<CardData>();

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

		public virtual Action<CardModel> AddCard(CardData card, bool isFirstCard)
		{
			Cards.Add(card);
			Player.AddCard(card, isFirstCard);
			return null;
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

		public virtual IEnumerator AwaitTurn(Action<PlayerAction> action, IGetTurnContext context)
		{
			yield return Player.AwaitTurn(action, context);
		}
	}
}
