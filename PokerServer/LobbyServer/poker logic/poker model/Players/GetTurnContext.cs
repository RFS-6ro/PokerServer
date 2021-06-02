﻿namespace TexasHoldem.Logic.Players
{
	using PokerSynchronisation;
	using System.Collections.Generic;
	using TexasHoldem.Logic.GameMechanics;

	public class GetTurnContext : IGetTurnContext
	{
		public GameRoundType RoundType { get; }

		public IReadOnlyCollection<PlayerActionAndName> PreviousRoundActions { get; }

		public int SmallBlind { get; }

		public int MoneyLeft { get; }

		public int CurrentPot { get; }

		public int MyMoneyInTheRound { get; }

		public int CurrentMaxBet { get; }

		public bool CanCheck => MyMoneyInTheRound == CurrentMaxBet;

		public bool CanRaise => MinRaise > 0 && MoneyLeft > MoneyToCall;

		public int MoneyToCall
		{
			get
			{
				var temp = CurrentMaxBet - MyMoneyInTheRound;
				if (temp >= MoneyLeft)
				{
					// The player does not have enough money to make a full call
					return MoneyLeft;
				}
				else
				{
					return temp;
				}
			}
		}

		public bool IsAllIn => MoneyLeft <= 0;

		public int MinRaise { get; }

		public Pot MainPot { get; }

		public IReadOnlyCollection<Pot> SidePots { get; }

		public float TimeForTurn { get; }

		public GetTurnContext(
			GameRoundType roundType,
			IReadOnlyCollection<PlayerActionAndName> previousRoundActions,
			int smallBlind,
			int moneyLeft,
			int currentPot,
			int myMoneyInTheRound,
			int currentMaxBet,
			int minRaise,
			float timeForTurn,
			Pot mainPot,
			List<Pot> sidePots)
		{
			RoundType = roundType;
			PreviousRoundActions = previousRoundActions;
			SmallBlind = smallBlind;
			MoneyLeft = moneyLeft;
			CurrentPot = currentPot;
			MyMoneyInTheRound = myMoneyInTheRound;
			CurrentMaxBet = currentMaxBet;
			MinRaise = minRaise;
			MainPot = mainPot;
			SidePots = sidePots;
			TimeForTurn = timeForTurn;
		}
	}
}