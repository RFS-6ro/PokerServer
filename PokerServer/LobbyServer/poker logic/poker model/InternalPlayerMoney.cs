namespace LobbyServer.pokerlogic.GameMechanics
{
	using PokerSynchronisation;
	using System;
	using LobbyServer.pokerlogic.pokermodel.Players;

	public class InternalPlayerMoney
	{
		public InternalPlayerMoney(int startMoney)
		{
			Money = startMoney;
			NewHand();
			NewRound(GameRoundType.Posting);
		}

		// Player money in the game
		public int Money { get; set; }

		// The amount of money the player is currently put in the pot
		public int CurrentlyInPot { get; private set; }

		// The amount of money the player is currently bet
		public int CurrentRoundBet { get; private set; }

		// False when player folds
		public bool InHand { get; private set; }

		// Player action is expected (some other player raised)
		public bool ShouldPlayInRound { get; set; }

		public PlayerAction LastPlayerAction { get; set; }

		public void NewHand()
		{
			CurrentlyInPot = 0;
			CurrentRoundBet = 0;
			InHand = true;
			ShouldPlayInRound = true;
		}

		public void NewRound(GameRoundType type)
		{
			if ((int)type != (int)GameRoundType.PreFlop)
			{
				CurrentRoundBet = 0;
			}

			if (InHand && Money > 0)
			{
				ShouldPlayInRound = true;
			}
		}



		// TODO: Currently there is no limit in the raise amount as long as it is positive number
		public PlayerAction DoPlayerAction(PlayerAction action, int maxMoneyPerPlayer)
		{
			PlayerAction resultAction = action;
			if (action.Type == TurnType.Post)
			{
				if (Money >= action.Money)
				{
					PlaceMoney(action.Money);
				}
				else
				{
					PlaceMoney(Money);
				}
			}
			else if (action.Type == TurnType.Raise)
			{
				CallTo(maxMoneyPerPlayer);

				if (Money <= 0)
				{
					resultAction = PlayerAction.CheckOrCall();
				}

				if (Money > action.Money)
				{
					PlaceMoney(action.Money);
				}
				else
				{
					// All-in
					action.Money = Money;
					PlaceMoney(action.Money);
				}
			}
			else if (action.Type == TurnType.Call)
			{
				CallTo(maxMoneyPerPlayer);
			}
			else //// TurnType.Fold
			{
				InHand = false;
				ShouldPlayInRound = false;
			}

			LastPlayerAction = resultAction;

			return resultAction;
		}

		public void NormalizeBets(int moneyPerPlayer)
		{
			if (moneyPerPlayer < CurrentRoundBet)
			{
				var diff = CurrentRoundBet - moneyPerPlayer;
				PlaceMoney(-diff);
			}
		}

		private void PlaceMoney(int money)
		{
			CurrentRoundBet += money;
			CurrentlyInPot += money;
			Money -= money;
		}

		private void CallTo(int maxMoneyPerPlayer)
		{
			var moneyToPay = Math.Min(CurrentRoundBet + Money, maxMoneyPerPlayer);
			var diff = moneyToPay - CurrentRoundBet;
			PlaceMoney(diff);
		}
	}
}
