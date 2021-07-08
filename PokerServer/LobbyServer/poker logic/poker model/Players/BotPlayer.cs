using System;
using System.Threading.Tasks;
using LobbyServer.pokerlogic.Extensions;

namespace LobbyServer.pokerlogic.pokermodel.Players
{
	public class BotPlayer : BasePlayer
	{
		public BotPlayer(Guid guid, string name, int buyIn)
		{
			Guid = guid;
			Name = name; BuyIn = buyIn;
		}

		public override string Name { get; }

		public override int BuyIn { get; }

		public override PlayerAction PostingBlind(IPostingBlindContext context)
		{
			return context.BlindAction;
		}

		public async override Task<PlayerAction> GetTurn(IGetTurnContext context)
		{
			await Task.Delay(RandomProvider.Next(1000, 3000));

			//PlayerAction action;
			var chanceForAction = RandomProvider.Next(1, 101);
			if (chanceForAction == 1 && context.MoneyLeft > 0)
			{
				// All-in
				return PlayerAction.Raise(context.MoneyLeft - context.MoneyToCall);
			}

			if (chanceForAction <= 15)
			{
				if (context.CanRaise)
				{
					if (context.MinRaise + context.CurrentMaxBet > context.MoneyLeft)
					{
						//Debug.Log("All in" + Name);
						// All-in
						return PlayerAction.Raise(context.MoneyLeft - context.MoneyToCall);
					}
					else
					{
						//Debug.Log("Min raise" + Name);
						// Minimum raise
						return PlayerAction.Raise(context.MinRaise);
					}
				}
				else
				{
					//Debug.Log("check or call" + Name);
					return PlayerAction.CheckOrCall();
				}
			}

			// Play safe
			if (context.CanCheck)
			{
				//Debug.Log("check or call" + Name);
				return PlayerAction.CheckOrCall();
			}

			if (chanceForAction <= 60)
			{
				// Call
				//Debug.Log("check or call" + Name);
				return PlayerAction.CheckOrCall();
			}
			else
			{
				// Fold
				//Debug.Log("fold" + Name);
				return PlayerAction.Fold();
			}

			//Debug.Log(action + Name);
			//return action;
		}

		public override async Task<PlayerAction> AwaitTurn(IGetTurnContext context)
		{
			return await GetTurn(context);
		}
	}
}
