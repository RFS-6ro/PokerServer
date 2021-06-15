﻿namespace LobbyServer.pokerlogic.GameMechanics
{
	using System.Collections.Generic;
	using System.Linq;
	using LobbyServer.pokerlogic.pokermodel.Players;


	public class PotCreator<TDECORATOR>
		where TDECORATOR : PlayerDecorator, new()
	{
		private IList<TDECORATOR> _players;

		public PotCreator(IList<TDECORATOR> players)
		{
			_players = players;
		}

		public Pot MainPot
		{
			get
			{
				var levels = Levels();
				var upperLimit = levels.First();
				return Create(0, upperLimit);
			}
		}

		public List<Pot> SidePots
		{
			get
			{
				var pots = new List<Pot>();
				var levels = Levels();

				if (levels.Count > 1)
				{
					var list = levels.ToList();

					for (int i = 0; i < list.Count - 1; i++)
					{
						var pot = Create(list[i], list[i + 1]);

						if (pot.AmountOfMoney != 0)
						{
							pots.Add(pot);
						}
					}
				}

				return pots;
			}
		}

		private SortedSet<int> Levels()
		{
			var levels = new SortedSet<int> { int.MaxValue };

			foreach (var item in _players)
			{
				if (item.PlayerMoney.Money <= 0)
				{
					levels.Add(item.PlayerMoney.CurrentlyInPot);
				}
			}

			return levels;
		}

		private Pot Create(int lowerLimit, int upperLimit)
		{
			var amountOfMoney = 0;
			var activePlayer = new List<string>();

			foreach (var item in _players)
			{
				if (item.PlayerMoney.CurrentlyInPot > lowerLimit && item.PlayerMoney.CurrentlyInPot <= upperLimit)
				{
					amountOfMoney += item.PlayerMoney.CurrentlyInPot - lowerLimit;

					if (item.PlayerMoney.InHand)
					{
						activePlayer.Add(item.Name);
					}
				}
				else if (item.PlayerMoney.CurrentlyInPot > upperLimit)
				{
					amountOfMoney += upperLimit - lowerLimit;

					if (item.PlayerMoney.InHand)
					{
						activePlayer.Add(item.Name);
					}
				}
			}

			return new Pot(amountOfMoney, activePlayer);
		}
	}
}
