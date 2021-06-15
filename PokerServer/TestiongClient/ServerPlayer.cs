using System;
using System.Threading.Tasks;

namespace TestingClient
{
	public class ServerPlayer
	{
		private readonly int Row;
		public Guid Guid { get; }

		public string Name { get; }

		public int BuyIn { get; }

		public ServerPlayer(Guid guid, string name, int row, int buyIn = -1)
		{
			Name = name;
			Guid = guid;
			Row = row;
			BuyIn = buyIn;
		}

		public void PostingBlind(int amount)
		{
			//TODO: Display posting blind
		}

		//public async Task<PlayerAction> GetTurn(IGetTurnContext context)
		public async Task GetTurn()
		{
			if (!context.CanRaise)
			{
				DrawRestrictedPlayerOptions(context.MoneyToCall);
			}
			else
			{
				DrawPlayerOptions(context.MoneyToCall);
			}

			while (true)
			{
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
						continue;
					}
					int amount = await RaiseAmount(context.MoneyLeft, context.MinRaise, context.MoneyToCall, context.MyMoneyInTheRound);
					action = PlayerAction.Raise(amount);
					break;
				case ConsoleKey.F:
					action = PlayerAction.Fold();
					break;
				case ConsoleKey.A:
					if (!context.CanRaise)
					{
						continue;
					}

					action = context.MoneyLeft > 0
								 ? PlayerAction.Raise(context.MoneyLeft - context.MoneyToCall)
								 : PlayerAction.CheckOrCall();
					break;
				}

				if (action != null)
				{
					return action;
				}
			}
			await Task.Delay(3000);


		}

		private int _playerRaiseAmount = -1;

		public void SetPlayerRaiseAmount(int amount)
		{
			//TODORECEIVE:
			_playerRaiseAmount = amount;
		}

		private async Task<int> RaiseAmount(int moneyLeft, int minRaise, int moneyToCall, int myMoneyInTheRound)
		{
			int amount;
			var wholeMinRaise = minRaise + myMoneyInTheRound + moneyToCall;
			if (wholeMinRaise >= moneyLeft + myMoneyInTheRound)
			{
				// Instant All-In
				amount = moneyLeft - moneyToCall;
			}
			else
			{
				var perfix = $"Raise amount [{wholeMinRaise}-{moneyLeft + myMoneyInTheRound}]:";

				do
				{
					ConsoleHelper.WriteOnConsole(Row + 2, 2, new string(' ', Console.WindowWidth - 3));
					ConsoleHelper.WriteOnConsole(Row + 2, 2, perfix);

					//var text = ConsoleHelper.UserInput(Row + 2, perfix.Length + 3);

					while (_playerRaiseAmount == -1)
					{
						await Task.Yield();
					}

					int result = _playerRaiseAmount;

					//if (int.TryParse(text, out result))
					//{
					if (result < wholeMinRaise)
					{
						amount = minRaise;
						break;
					}
					else if (result >= moneyLeft + myMoneyInTheRound)
					{
						// Raise All-in
						amount = moneyLeft - moneyToCall;
						break;
					}

					amount = result - (myMoneyInTheRound + moneyToCall);
					break;
					//}
				}
				while (true);
			}

			_playerRaiseAmount = -1;

			return amount;
		}
	}
}
