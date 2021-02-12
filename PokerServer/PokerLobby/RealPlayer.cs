using PokerSynchronisation;
using TexasHoldem.Logic.Players;

namespace PokerLobby
{
	public class RealPlayer : BasePlayer
	{
		public override string Name { get; }

		public override int BuyIn { get; } = -1;

		public bool IsReady { get; set; } = false;

		public readonly int ServerId;

		private TurnType _currentType = TurnType.None;
		private int _raiseAmount = -1;

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
			PlayerAction action = null;

			if (_currentType != TurnType.None)
			{
				switch (_currentType)
				{
				case TurnType.Raise:
					if (!context.CanRaise)
					{
						ResetExternalInput();
						return null;
					}

					action = PlayerAction.Raise(RaiseAmount(context.MoneyLeft, context.MinRaise, context.MoneyToCall, context.MyMoneyInTheRound));
					ResetExternalInput();
					break;
				case TurnType.Fold:
					action = PlayerAction.Fold();
					ResetExternalInput();
					break;
				case TurnType.AllIn:
					if (!context.CanRaise)
					{
						ResetExternalInput();
						return null;
					}

					action = context.MoneyLeft > 0
								 ? PlayerAction.Raise(context.MoneyLeft - context.MoneyToCall)
								 : PlayerAction.CheckOrCall();
					ResetExternalInput();
					break;
				default:
					ResetExternalInput();
					break;
				}
			}

			return action;


			/*

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
			*/
		}

		public void ApplyExternalInput(TurnType type, int amount)
		{
			_currentType = type;
			_raiseAmount = amount;
		}

		private void ResetExternalInput()
		{
			_currentType = TurnType.None;
			_raiseAmount = -1;
		}

		private int RaiseAmount(int moneyLeft, int minRaise, int moneyToCall, int myMoneyInTheRound)
		{
			var wholeMinRaise = minRaise + myMoneyInTheRound + moneyToCall;
			if (wholeMinRaise >= moneyLeft + myMoneyInTheRound)
			{
				// Instant All-In
				return moneyLeft - moneyToCall;
			}

			if (_raiseAmount < wholeMinRaise)
			{
				return minRaise;
			}
			else if (_raiseAmount >= moneyLeft + myMoneyInTheRound)
			{
				// Raise All-in
				return moneyLeft - moneyToCall;
			}

			return _raiseAmount - (myMoneyInTheRound + moneyToCall);
		}
	}
}
