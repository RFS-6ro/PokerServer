using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Network;
using PokerSynchronisation;
using TexasHoldem.Logic.GameMechanics;
using TexasHoldem.Logic.Players;

namespace PokerLobby
{
	public class BettingLogicHandler
	{
		private readonly int _initialPlayerIndex;

		private readonly IList<RealPlayerDecorator> _players;

		private readonly int _smallBlind;

		private PotCreator<RealPlayerDecorator> _potCreator;

		private MinRaise _minRaise;

		public BettingLogicHandler(IList<RealPlayerDecorator> players, int smallBlind)
		{
			Random rnd = new Random();
			_initialPlayerIndex = players.Count == 2 ? 0 : 1;
			_players = players;
			_smallBlind = smallBlind;
			RoundBets = new List<PlayerActionAndName>();
			_potCreator = new PotCreator<RealPlayerDecorator>(_players);
			_minRaise = new MinRaise(_smallBlind);

#if DEBUG
			//CHECK: send dealer's server index event
			//CHECK: move dealerButton
			LobbySendHandle.DealerPosition(LobbyClient.Instance.Id, _players[_initialPlayerIndex].ServerId);
#endif
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

		public async Task Bet(GameRoundType gameRoundType)
		{
			UpdateBank();

			var playerIndex = gameRoundType == GameRoundType.PreFlop
				? _initialPlayerIndex
				: 1;

			if (gameRoundType == GameRoundType.PreFlop)
			{
				//PlaceBlinds();
				playerIndex = _initialPlayerIndex + 2;
			}
			else
			{
				RoundBets.Clear();
				_minRaise.Reset();
			}

			if (_players.Count(x => x.PlayerMoney.ShouldPlayInRound) <= 1)
			{
				return;
			}

			int safeCounter = 0;

			while (_players.Count(x => x.PlayerMoney.InHand) >= 2
				   && _players.Any(x => x.PlayerMoney.ShouldPlayInRound))
			{
				safeCounter++;

				if (safeCounter > 100)
				{
					return;
				}

				var player = _players[playerIndex % _players.Count];
				if (player.PlayerMoney.Money <= 0)
				{
					// Players who are all-in are not involved in betting round
					player.PlayerMoney.ShouldPlayInRound = false;
					playerIndex++;
					continue;
				}

				if (player.PlayerMoney.InHand == false || player.PlayerMoney.ShouldPlayInRound == false)
				{
					if (player.PlayerMoney.InHand == player.PlayerMoney.ShouldPlayInRound)
					{
						playerIndex++;
					}

					continue;
				}

				var maxMoneyPerPlayer = _players.Max(x => x.PlayerMoney.CurrentRoundBet);

				GetTurnContext turnContext = new GetTurnContext(
							gameRoundType,
							RoundBets.AsReadOnly(),
							_smallBlind,
							player.PlayerMoney.Money,
							Pot,
							player.PlayerMoney.CurrentRoundBet,
							maxMoneyPerPlayer,
							_minRaise.Amount(player.Name),
							MainPot,
							SidePots);

				int lastBet = player.PlayerMoney.CurrentRoundBet;
#if DEBUG
				//CHECK: Send start turn event
				LobbySendHandle.StartTurn(LobbyClient.Instance.Id, player.ServerId, turnContext.CanRaise);
#endif

				var action = await GettingTurn(player, turnContext, maxMoneyPerPlayer);

				action = player.PlayerMoney.DoPlayerAction(action, maxMoneyPerPlayer);

				if (lastBet <= player.PlayerMoney.CurrentRoundBet)
				{
#if DEBUG
					//CHECK: need bet animation
					LobbySendHandle.ShowPlayerBet(LobbyClient.Instance.Id, player.ServerId, player.PlayerMoney.CurrentRoundBet - lastBet);
#endif
				}

				RoundBets.Add(new PlayerActionAndName(player.Name, action));

				UpdateBank();

				if (action.Type == TurnType.Raise)
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

#if DEBUG
				//CHECK: Send end turn event
				LobbySendHandle.EndTurn(LobbyClient.Instance.Id, player.ServerId);
#endif

				await Task.Delay(500);
			}

			if (gameRoundType != GameRoundType.PreFlop)
			{
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

			await Task.Delay(500);

			UpdateBank();
		}

		private async Task<PlayerAction> GettingTurn(RealPlayerDecorator player, GetTurnContext turnContext, int awaitTime = 10000)
		{
			int timeLeft = awaitTime;
#if DEBUG
			//CHECK: send start turn timer event
			LobbySendHandle.TimerEvent(LobbyClient.Instance.Id, player.ServerId, true, timeLeft);
#endif
			int approveTimerDelta = 300;
			int askDelay = 50;

			PlayerAction action = null;

			while (action == null)
			{
				action = player.GetTurn(turnContext);
				await Task.Delay(askDelay);

				approveTimerDelta -= askDelay;
				timeLeft -= askDelay;

				if (timeLeft <= 0)
				{
#if DEBUG
					//CHECK: send stop turn timer event
					LobbySendHandle.TimerEvent(LobbyClient.Instance.Id, player.ServerId, false, timeLeft);
#endif
					return PlayerAction.Fold();
				}

				if (approveTimerDelta <= 0)
				{
					approveTimerDelta = 300;
#if DEBUG
					//CHECK: send timer approvance event
					LobbySendHandle.TimerEvent(LobbyClient.Instance.Id, player.ServerId, true, timeLeft);
#endif
				}
			}

#if DEBUG
			//CHECK: send stop turn timer event
			LobbySendHandle.TimerEvent(LobbyClient.Instance.Id, player.ServerId, false, timeLeft);
#endif
			return action;
		}

		public async Task PlaceBlinds()
		{
			await Task.Delay(300);
#if DEBUG
			//CHECK: send small blind posting event
			LobbySendHandle.ShowPlayerBet(LobbyClient.Instance.Id, _players[_initialPlayerIndex].ServerId, _smallBlind);
#endif
			// Small blind
			RoundBets.Add(
				new PlayerActionAndName(
					_players[_initialPlayerIndex].Name,
					_players[_initialPlayerIndex].PostingBlind(
						new PostingBlindContext(
							_players[_initialPlayerIndex].PlayerMoney.DoPlayerAction(PlayerAction.Post(_smallBlind), 0),
							0,
							_players[_initialPlayerIndex].PlayerMoney.Money))));

			await Task.Delay(300);

			// Big blind
#if DEBUG
			//CHECK: send big blind posting event
			LobbySendHandle.ShowPlayerBet(LobbyClient.Instance.Id, _players[_initialPlayerIndex + 1].ServerId, 2 * _smallBlind);
#endif

			RoundBets.Add(
				new PlayerActionAndName(
					_players[_initialPlayerIndex + 1].Name,
					_players[_initialPlayerIndex + 1].PostingBlind(
						new PostingBlindContext(
							_players[_initialPlayerIndex + 1].PlayerMoney.DoPlayerAction(PlayerAction.Post(2 * _smallBlind), 0),
							Pot,
							_players[_initialPlayerIndex + 1].PlayerMoney.Money))));
			await Task.Delay(500);
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

		private void UpdateBank()
		{
#if DEBUG
			//CHECK: Update bank and send event
			LobbySendHandle.ShowBank(LobbyClient.Instance.Id, Pot);
#endif
		}
	}
}
