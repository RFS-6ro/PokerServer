using System;
using System.Threading.Tasks;
using PokerSynchronisation;
using TexasHoldem.Logic.Extensions;
using TexasHoldem.Logic.Players;
//using TexasHoldem.UI.Console;

namespace PokerLobby
{
	public class RealPlayer : BasePlayer
	{
		public override string Name { get; }

		public override int BuyIn { get; } = -1;

		public bool IsReady { get; set; } = false;

		public readonly int ServerId;

		public RealPlayer(string name, int serverId) : base()
		{
			Name = name;
			ServerId = serverId;
		}

		public override PlayerAction PostingBlind(IPostingBlindContext context)
		{
			return context.BlindAction;
		}

		public override PlayerAction GetTurn(IGetTurnContext context)
		{
			/*
			//TODO: replace by TurnType
			var key = Console.ReadKey(true);
			PlayerAction action = null;
			
			switch (key.Key)
			{
			case ConsoleKey.C:
				action = PlayerAction.CheckOrCall();
				break;
			case ConsoleKey.R:
				if (!context.CanRaise)
				{
					return null;
				}

				action = PlayerAction.Raise(RaiseAmount(context.MoneyLeft, context.MinRaise, context.MoneyToCall, context.MyMoneyInTheRound));
				break;
			case ConsoleKey.F:
				action = PlayerAction.Fold();
				break;
			case ConsoleKey.A:
				if (!context.CanRaise)
				{
					return null;
				}

				action = context.MoneyLeft > 0
							 ? PlayerAction.Raise(context.MoneyLeft - context.MoneyToCall)
							 : PlayerAction.CheckOrCall();
				break;
			}

			return action;
			*/

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
						// All-in
						return PlayerAction.Raise(context.MoneyLeft - context.MoneyToCall);
					}
					else
					{
						// Minimum raise
						return PlayerAction.Raise(context.MinRaise);
					}
				}
				else
				{
					return PlayerAction.CheckOrCall();
				}
			}

			// Play safe
			if (context.CanCheck)
			{
				return PlayerAction.CheckOrCall();
			}

			if (chanceForAction <= 60)
			{
				// Call
				return PlayerAction.CheckOrCall();
			}
			else
			{
				// Fold
				return PlayerAction.Fold();
			}
		}

		private int RaiseAmount(int moneyLeft, int minRaise, int moneyToCall, int myMoneyInTheRound)
		{
			var wholeMinRaise = minRaise + myMoneyInTheRound + moneyToCall;
			if (wholeMinRaise >= moneyLeft + myMoneyInTheRound)
			{
				// Instant All-In
				return moneyLeft - moneyToCall;
			}

			do
			{
				var text = Console.ReadLine();
				//TODO: paste raise amount here
				int result;

				if (int.TryParse(text, out result))
				{
					if (result < wholeMinRaise)
					{
						return minRaise;
					}
					else if (result >= moneyLeft + myMoneyInTheRound)
					{
						// Raise All-in
						return moneyLeft - moneyToCall;
					}

					return result - (myMoneyInTheRound + moneyToCall);
				}
			}
			while (true);
		}
	}
}
