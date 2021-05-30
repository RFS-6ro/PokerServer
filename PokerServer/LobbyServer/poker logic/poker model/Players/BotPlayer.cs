using System;
using System.Collections;
using TexasHoldem.Logic.Extensions;
using TexasHoldem.Logic.Players;
using UnityEngine;

namespace GameCore.Poker.Model.Player
{
	public class BotPlayer : BasePlayer
	{
		public override string Name => "DummyPlayer_" + Guid.NewGuid();

		public override int BuyIn => -1;

		public override TexasHoldem.Logic.Players.PlayerAction PostingBlind(IPostingBlindContext context)
		{
			return context.BlindAction;
		}

		public override TexasHoldem.Logic.Players.PlayerAction GetTurn(IGetTurnContext context)
		{
			TexasHoldem.Logic.Players.PlayerAction action;
			var chanceForAction = RandomProvider.Next(1, 101);
			if (chanceForAction == 1 && context.MoneyLeft > 0)
			{
				// All-in
				action = TexasHoldem.Logic.Players.PlayerAction.Raise(context.MoneyLeft - context.MoneyToCall);
			}

			if (chanceForAction <= 15)
			{
				if (context.CanRaise)
				{
					if (context.MinRaise + context.CurrentMaxBet > context.MoneyLeft)
					{
						//Debug.Log("All in" + Name);
						// All-in
						action = TexasHoldem.Logic.Players.PlayerAction.Raise(context.MoneyLeft - context.MoneyToCall);
					}
					else
					{
						//Debug.Log("Min raise" + Name);
						// Minimum raise
						action = TexasHoldem.Logic.Players.PlayerAction.Raise(context.MinRaise);
					}
				}
				else
				{
					//Debug.Log("check or call" + Name);
					action = TexasHoldem.Logic.Players.PlayerAction.CheckOrCall();
				}
			}

			// Play safe
			if (context.CanCheck)
			{
				//Debug.Log("check or call" + Name);
				action = TexasHoldem.Logic.Players.PlayerAction.CheckOrCall();
			}

			if (chanceForAction <= 60)
			{
				// Call
				//Debug.Log("check or call" + Name);
				action = TexasHoldem.Logic.Players.PlayerAction.CheckOrCall();
			}
			else
			{
				// Fold
				//Debug.Log("fold" + Name);
				action = TexasHoldem.Logic.Players.PlayerAction.Fold();
			}

			//Debug.Log(action + Name);
			return action;
		}

		public override IEnumerator AwaitTurn(Action<TexasHoldem.Logic.Players.PlayerAction> action, IGetTurnContext context)
		{
			yield return new WaitForSeconds(UnityEngine.Random.Range(2f, 5f));

			action?.Invoke(GetTurn(context));
		}
	}
}
