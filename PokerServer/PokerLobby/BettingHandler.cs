using System;
using System.Collections.Generic;
using System.Linq;
using PokerSynchronisation;
using TexasHoldem.Logic.GameMechanics;
using TexasHoldem.Logic.Players;

namespace PokerLobby
{
	public class BettingHandler
	{
		private readonly int _initialPlayerIndex;

		private readonly IList<InternalPlayer> _players;

		private readonly int _smallBlind;

		private PotCreator _potCreator;

		private MinRaise _minRaise;

		public BettingHandler(IList<InternalPlayer> players, int smallBlind)
		{
			Random rnd = new Random();
			_initialPlayerIndex = players.Count == 2 ? 0 : 1;
			_players = players;
			_smallBlind = smallBlind;
			RoundBets = new List<PlayerActionAndName>();
			_potCreator = new PotCreator(_players);
			_minRaise = new MinRaise(_smallBlind);
		}

		public int Pot
		{
			get
			{
				return _players.Sum(x => x.PlayerMoney.CurrentlyInPot);
			}
		}

		public Pot MainPot
		{
			get
			{
				return _potCreator.MainPot;
			}
		}

		public List<Pot> SidePots
		{
			get
			{
				return _potCreator.SidePots;
			}
		}

		public List<PlayerActionAndName> RoundBets { get; }

		public void Bet(GameRoundType gameRoundType)
		{
			RoundBets.Clear();
			_minRaise.Reset();
			var playerIndex = gameRoundType == GameRoundType.PreFlop
				? _initialPlayerIndex
				: 1;

			if (gameRoundType == GameRoundType.PreFlop)
			{
				PlaceBlinds();
				playerIndex = _initialPlayerIndex + 2;
			}

			if (_players.Count(x => x.PlayerMoney.ShouldPlayInRound) <= 1)
			{
				return;
			}

			while (_players.Count(x => x.PlayerMoney.InHand) >= 2
				   && _players.Any(x => x.PlayerMoney.ShouldPlayInRound))
			{
				var player = _players[playerIndex % _players.Count];
				if (player.PlayerMoney.Money <= 0)
				{
					// Players who are all-in are not involved in betting round
					player.PlayerMoney.ShouldPlayInRound = false;
					playerIndex++;
					continue;
				}

				if (!player.PlayerMoney.InHand || !player.PlayerMoney.ShouldPlayInRound)
				{
					if (player.PlayerMoney.InHand == player.PlayerMoney.ShouldPlayInRound)
					{
						playerIndex++;
					}

					continue;
				}

				var maxMoneyPerPlayer = _players.Max(x => x.PlayerMoney.CurrentRoundBet);
				var action =
					player.GetTurn(
						new GetTurnContext(
							gameRoundType,
							RoundBets.AsReadOnly(),
							_smallBlind,
							player.PlayerMoney.Money,
							Pot,
							player.PlayerMoney.CurrentRoundBet,
							maxMoneyPerPlayer,
							_minRaise.Amount(player.Name),
							MainPot,
							SidePots));

				action = player.PlayerMoney.DoPlayerAction(action, maxMoneyPerPlayer);
				RoundBets.Add(new PlayerActionAndName(player.Name, action));

				if (action.Type == PlayerActionType.Raise)
				{
					// When raising, all players are required to do action afterwards in current round
					foreach (var playerToUpdate in _players)
					{
						playerToUpdate.PlayerMoney.ShouldPlayInRound = playerToUpdate.PlayerMoney.InHand ? true : false;
					}
				}

				_minRaise.Update(player.Name, maxMoneyPerPlayer, player.PlayerMoney.CurrentRoundBet);
				player.PlayerMoney.ShouldPlayInRound = false;
				playerIndex++;
				//Thread.Sleep(1000);
			}

			if (_players.Count == 2)
			{
				// works only for heads-up
				ReturnMoneyInCaseOfAllIn();
			}
			else
			{
				ReturnMoneyInCaseUncalledBet();
			}
		}

		private void PlaceBlinds()
		{
			// Small blind
			RoundBets.Add(
				new PlayerActionAndName(
					_players[_initialPlayerIndex].Name,
					_players[_initialPlayerIndex].PostingBlind(
						new PostingBlindContext(
							_players[_initialPlayerIndex].PlayerMoney.DoPlayerAction(PlayerAction.Post(_smallBlind), 0),
							0,
							_players[_initialPlayerIndex].PlayerMoney.Money))));

			// Big blind
			RoundBets.Add(
				new PlayerActionAndName(
					_players[_initialPlayerIndex + 1].Name,
					_players[_initialPlayerIndex + 1].PostingBlind(
						new PostingBlindContext(
							_players[_initialPlayerIndex + 1].PlayerMoney.DoPlayerAction(PlayerAction.Post(2 * _smallBlind), 0),
							Pot,
							_players[_initialPlayerIndex + 1].PlayerMoney.Money))));
		}

		private void ReturnMoneyInCaseOfAllIn()
		{
			var minMoneyPerPlayer = _players.Min(x => x.PlayerMoney.CurrentRoundBet);
			foreach (var player in _players)
			{
				player.PlayerMoney.NormalizeBets(minMoneyPerPlayer);
			}
		}

		private void ReturnMoneyInCaseUncalledBet()
		{
			var group = _players.GroupBy(x => x.PlayerMoney.CurrentRoundBet).OrderByDescending(k => k.Key);
			if (group.First().Count() == 1)
			{
				group.First().First().PlayerMoney.NormalizeBets(group.ElementAt(1).First().PlayerMoney.CurrentRoundBet);
			}
		}
	}
}
