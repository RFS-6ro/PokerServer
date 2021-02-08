using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TexasHoldem.Logic.GameMechanics;
using TexasHoldem.Logic.Players;

namespace PokerLobby
{
	public class TexasHoldemGameLogic<TDECORATOR>
		where TDECORATOR : PlayerDecorator, new()
	{
		public static readonly int[] SmallBlinds =
		{
			1, 2, 3, 5, 10, 15, 20, 25, 30, 40, 50, 60, 80, 100, 150, 200, 300,
			400, 500, 600, 800, 1000, 1500, 2000, 3000, 4000, 5000, 6000, 8000,
			10000, 15000, 20000, 30000, 40000, 50000, 60000, 80000, 100000,
		};

		private readonly ICollection<TDECORATOR> _players;

		private int _initialMoney;

		public int HandsPlayed { get; private set; }

		public TexasHoldemGameLogic(IList<IPlayer> players, int initialMoney = 200)
			: this((ICollection<IPlayer>)players, initialMoney)
		{
			// Ensure the players have unique names
			var duplicateNames = players.GroupBy(x => x)
				.Where(group => group.Count() > 1)
				.Select(group => group.Key.Name);

			if (duplicateNames.Count() > 0)
			{
				throw new ArgumentException($"Players have the same name: \"{string.Join(" ", duplicateNames.ToArray())}\"");
			}
		}

		private TexasHoldemGameLogic(ICollection<IPlayer> players, int initialMoney = 1000)
		{
			if (players == null)
			{
				throw new ArgumentNullException(nameof(players));
			}

			if (players.Count < 2 || players.Count > 7)
			{
				throw new ArgumentOutOfRangeException(nameof(players), "The number of players must be from 2 to 7");
			}

			if (initialMoney <= 0 || initialMoney > 200000)
			{
				throw new ArgumentOutOfRangeException(nameof(initialMoney), "Initial money should be greater than 0 and less than 200000");
			}

			_players = new List<TDECORATOR>(players.Count);
			foreach (var item in players)
			{
				TDECORATOR player = new TDECORATOR();
				player.SetPlayer(item);
				_players.Add(player);
			}

			_initialMoney = initialMoney;
			HandsPlayed = 0;
		}

		public async Task<IPlayer> Start()
		{
			await Task.Delay(500);

			var playerNames = _players.Select(x => x.Name).ToList().AsReadOnly();
			foreach (var player in _players)
			{
				player.StartGame(new StartGameContext(playerNames, player.BuyIn == -1 ? _initialMoney : player.BuyIn));
			}

			await PlayGame();

			var winner = _players.WithMoney().FirstOrDefault();
			foreach (var player in _players)
			{
				player.EndGame(new EndGameContext(winner.Name));
			}

			return winner;
		}

		private void Rebuy()
		{
			var playerNames = _players.Select(x => x.Name).ToList().AsReadOnly();
			foreach (var player in _players)
			{
				if (player.PlayerMoney.Money <= 0)
				{
					player.StartGame(new StartGameContext(playerNames, player.BuyIn == -1 ? _initialMoney : player.BuyIn));
				}
			}
		}

		private async Task PlayGame()
		{
			var shifted = _players.ToList();

			int shuffleNumber = new Random().Next(0, _players.Count - 1);

			for (int i = 0; i < shuffleNumber; i++)
			{
				shifted = shifted.WithMoney().Cast<TDECORATOR>().ToList();
				shifted.Add(shifted.First());
				shifted.RemoveAt(0);
			}

			// While at least two players have money
			while (_players.WithMoney().Count() > 1)
			{
				HandsPlayed++;

				// Every 10 hands the blind increases
				var smallBlind = SmallBlinds[(HandsPlayed - 1) / 10];
				//var smallBlind = SmallBlinds[0];

				// Players are shifted in order of priority to make a move
				shifted = shifted.WithMoney().Cast<TDECORATOR>().ToList();
				shifted.Add(shifted.First());
				shifted.RemoveAt(0);

				// Rotate players
				DealerLogic<TDECORATOR> dealer = new DealerLogic<TDECORATOR>(shifted, HandsPlayed, smallBlind);

				await dealer.Play();

				//Rebuy();
			}
		}
	}
}
