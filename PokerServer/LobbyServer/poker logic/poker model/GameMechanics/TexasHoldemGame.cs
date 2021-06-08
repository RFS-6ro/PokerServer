using LobbyServer.pokerlogic.controllers;
using LobbyServer.pokerlogic.pokermodel.Players;
using PokerSynchronisation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TexasHoldem.Logic.GameMechanics;
using TexasHoldem.Logic.Players;

namespace GameCore.Poker.Model
{
	public class TexasHoldemGame<TDECORATOR> where TDECORATOR : PlayerDecorator, new()
	{
		protected static readonly int[] SmallBlinds =
		{
			1, 2, 3, 5, 10, 15, 20, 25, 30, 40, 50, 60, 80, 100, 150, 200, 300,
			400, 500, 600, 800, 1000, 1500, 2000, 3000, 4000, 5000, 6000, 8000,
			10000, 15000, 20000, 30000, 40000, 50000, 60000, 80000, 100000,
		};

		private readonly ICollection<TDECORATOR> allPlayers;

		private readonly TableViewModel _tableViewModel;

		private int initialMoney;

		public int HandsPlayed { get; private set; }

		public TexasHoldemGame(IList<TDECORATOR> players, TableViewModel tableViewModel, int initialMoney = 200)
			: this(players, initialMoney)
		{
			// Ensure the players have unique names
			var duplicateNames = players.GroupBy(x => x)
				.Where(group => group.Count() > 1)
				.Select(group => group.Key.Name);

			if (tableViewModel == null)
			{
				throw new ArgumentNullException();
			}

			if (duplicateNames.Count() > 0)
			{
				throw new ArgumentException($"Players have the same name: \"{string.Join(" ", duplicateNames.ToArray())}\"");
			}

			_tableViewModel = tableViewModel;
		}

		private TexasHoldemGame(ICollection<TDECORATOR> players, int initialMoney = 1000)
		{
			if (players == null)
			{
				throw new ArgumentNullException(nameof(players));
			}

			if (players.Count < 2 || players.Count > 9)
			{
				throw new ArgumentOutOfRangeException(nameof(players), $"The number of players must be from 2 to {9}");
			}

			if (initialMoney <= 0 || initialMoney > 200000)
			{
				throw new ArgumentOutOfRangeException(nameof(initialMoney), "Initial money should be greater than 0 and less than 200000");
			}

			allPlayers = new List<TDECORATOR>(players.Count);
			foreach (var player in players)
			{
				//TDECORATOR player = new TDECORATOR(chairs[0]);
				//player.SetPlayer(item);
				allPlayers.Add(player);
			}

			this.initialMoney = initialMoney;
			HandsPlayed = 0;
		}

		public async Task<TDECORATOR> Start()
		{
			var playerNames = allPlayers.Select(x => x.Name).ToList().AsReadOnly();
			foreach (var player in allPlayers)
			{
				player.StartGame(new StartGameContext(playerNames, player.BuyIn == -1 ? initialMoney : player.BuyIn));
			}

			await PlayGame();

			var winner = allPlayers.WithMoney().FirstOrDefault();

			foreach (var player in allPlayers)
			{
				player.EndGame(new EndGameContext(winner.Name));
			}

			return winner;
		}

		private void Rebuy()
		{
			var playerNames = allPlayers.Select(x => x.Name).ToList().AsReadOnly();
			foreach (var player in allPlayers)
			{
				if (player.PlayerMoney.Money <= 0)
				{
					player.StartGame(new StartGameContext(playerNames, player.BuyIn == -1 ? initialMoney : player.BuyIn));
				}
			}
		}

		private async Task PlayGame()
		{
			var shifted = allPlayers.ToList();

			int shuffleNumber = new Random().Next(0, allPlayers.Count - 1);

			for (int i = 0; i < shuffleNumber; i++)
			{
				shifted = shifted.WithMoney().Cast<TDECORATOR>().ToList();
				shifted.Add(shifted.First());
				shifted.RemoveAt(0);
			}

			// While at least two players have money
			while (allPlayers.WithMoney().Count() > 1)
			{
				HandsPlayed++;

				//Every 10 hands the blind increases
				var smallBlind = SmallBlinds[(HandsPlayed - 1) / 10];
				//var smallBlind = SmallBlinds[0];

				// Players are shifted in order of priority to make a move
				shifted = shifted.WithMoney().Cast<TDECORATOR>().ToList();
				shifted.Add(shifted.First());
				shifted.RemoveAt(0);

				// Rotate players
				HandLogic<TDECORATOR> hand = new HandLogic<TDECORATOR>(shifted, HandsPlayed, smallBlind, _tableViewModel);

				await hand.Play();

				//Rebuy();
			}
		}
	}
}
