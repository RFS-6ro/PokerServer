using System;
using System.Threading.Tasks;
using TexasHoldem.Logic.Extensions;
using TexasHoldem.Logic.Players;

namespace GameCore.Poker.Model.Player
{
	public class BotPlayer : BasePlayer
	{
		public override string Name => "DummyPlayer_" + Guid.NewGuid();

		public override int BuyIn => -1;

		public override PlayerAction PostingBlind(IPostingBlindContext context)
		{
			return context.BlindAction;
		}

		public override PlayerAction GetTurn(IGetTurnContext context)
		{
			PlayerAction action;
			var chanceForAction = RandomProvider.Next(1, 101);
			if (chanceForAction == 1 && context.MoneyLeft > 0)
			{
				// All-in
				action = PlayerAction.Raise(context.MoneyLeft - context.MoneyToCall);
			}

			if (chanceForAction <= 15)
			{
				if (context.CanRaise)
				{
					if (context.MinRaise + context.CurrentMaxBet > context.MoneyLeft)
					{
						//Debug.Log("All in" + Name);
						// All-in
						action = PlayerAction.Raise(context.MoneyLeft - context.MoneyToCall);
					}
					else
					{
						//Debug.Log("Min raise" + Name);
						// Minimum raise
						action = PlayerAction.Raise(context.MinRaise);
					}
				}
				else
				{
					//Debug.Log("check or call" + Name);
					action = PlayerAction.CheckOrCall();
				}
			}

			// Play safe
			if (context.CanCheck)
			{
				//Debug.Log("check or call" + Name);
				action = PlayerAction.CheckOrCall();
			}

			if (chanceForAction <= 60)
			{
				// Call
				//Debug.Log("check or call" + Name);
				action = PlayerAction.CheckOrCall();
			}
			else
			{
				// Fold
				//Debug.Log("fold" + Name);
				action = PlayerAction.Fold();
			}

			//Debug.Log(action + Name);
			return action;
		}

		public override async Task AwaitTurn(Action<PlayerAction> action, IGetTurnContext context)
		{
			await Task.Delay(new Random().Next(2000, 4000));

			action?.Invoke(GetTurn(context));
		}
	}
}
