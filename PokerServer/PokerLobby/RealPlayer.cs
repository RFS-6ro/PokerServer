using System;
using System.Threading.Tasks;
using PokerSynchronisation;
using TexasHoldem.Logic.Players;
using TexasHoldem.UI.Console;

namespace PokerLobby
{
	public class RealPlayer : BasePlayer
	{
		public override string Name { get; }

		public override int BuyIn { get; }

		public bool IsReady => true;

		public RealPlayer(string name) : base()
		{
			Name = name;
		}

		public override PlayerAction PostingBlind(IPostingBlindContext context)
		{
			return context.BlindAction;
		}

		public override PlayerAction GetTurn(IGetTurnContext context)
		{
			if (!context.CanRaise)
			{
				//Allow only check and fold
			}
			else
			{
				//Allow all turn types
			}

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
					//TODO: send wrong input event
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
					//TODO: send wrong input event
					return null;
				}

				action = context.MoneyLeft > 0
							 ? PlayerAction.Raise(context.MoneyLeft - context.MoneyToCall)
							 : PlayerAction.CheckOrCall();
				break;
			}

			return action;
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
