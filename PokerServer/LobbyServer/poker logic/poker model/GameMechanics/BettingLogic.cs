namespace TexasHoldem.Logic.GameMechanics
{
	using LobbyServer.pokerlogic.controllers;
	using LobbyServer.pokerlogic.pokermodel.Players;
	using PokerSynchronisation;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using TexasHoldem.Logic.Players;

	public class BettingLogic<TDECORATOR>
		where TDECORATOR : PlayerDecorator, new()
	{
		private readonly int initialPlayerIndex = 1;

		private readonly IList<TDECORATOR> allPlayers;

		private readonly int _smallBlind;

		private readonly TableViewModel _tableViewModel;

		private PotCreator<TDECORATOR> potCreator;

		private MinRaise minRaise;

		public BettingLogic(IList<TDECORATOR> players, int smallBlind, TableViewModel tableViewModel)
		{
			//initialPlayerIndex = players.Count == 2 ? 0 : 1;
			allPlayers = players;
			_smallBlind = smallBlind;
			RoundBets = new List<PlayerActionAndName>();
			potCreator = new PotCreator<TDECORATOR>(allPlayers);
			minRaise = new MinRaise(_smallBlind);
			_tableViewModel = tableViewModel;
		}

		public int Pot
		{
			get
			{
				return allPlayers.Sum(x => x.PlayerMoney.CurrentlyInPot);
			}
		}

		public Pot MainPot
		{
			get
			{
				return potCreator.MainPot;
			}
		}

		public List<Pot> SidePots
		{
			get
			{
				return potCreator.SidePots;
			}
		}

		public List<PlayerActionAndName> RoundBets { get; }

		public async Task Bet(GameRoundType gameRoundType)
		{
			int playerIndex = 1;

			if (gameRoundType == GameRoundType.PreFlop)
			{
				playerIndex = initialPlayerIndex + 2;
			}
			else
			{
				RoundBets.Clear();
				minRaise.Reset();
			}

			if (allPlayers.Count(x => x.PlayerMoney.ShouldPlayInRound) <= 1)
			{
				return;
			}

			while (allPlayers.Count(x => x.PlayerMoney.InHand) >= 2
				   && allPlayers.Any(x => x.PlayerMoney.ShouldPlayInRound))
			{
				var player = allPlayers[playerIndex % allPlayers.Count];
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

				var maxMoneyPerPlayer = allPlayers.Max(x => x.PlayerMoney.CurrentRoundBet);
				IGetTurnContext context = new GetTurnContext(
							gameRoundType,
							RoundBets.AsReadOnly(),
							_smallBlind,
							player.PlayerMoney.Money,
							Pot,
							player.PlayerMoney.CurrentRoundBet,
							maxMoneyPerPlayer,
							minRaise.Amount(player.Name),
							5f,
							MainPot,
							SidePots);

				player.ChairView.SetGameStateHolder(string.Empty);

				PlayerAction action = await player.AwaitTurn(context);
				//PlayerAction action = player.GetTurn(context);

				player.ChairView.SetGameStateHolder(action.ToString());

				action = player.PlayerMoney.DoPlayerAction(action, maxMoneyPerPlayer);
				player.ChairView?.SetMoneyView(player.PlayerMoney.Money);

				RoundBets.Add(new PlayerActionAndName(player.Name, action));

				//BetController.MoveBet(player.ChairView, player.PlayerMoney.CurrentRoundBet, player.ChairView);

				if (action.Type == TurnType.Fold)
				{
					player.ChairView.ClearCards();
				}

				if (action.Type == TurnType.Raise)
				{
					// When raising, all players are required to do action afterwards in current round
					foreach (var playerToUpdate in allPlayers)
					{
						playerToUpdate.PlayerMoney.ShouldPlayInRound = playerToUpdate.PlayerMoney.InHand ? true : false;
					}
				}

				minRaise.Update(player.Name, maxMoneyPerPlayer, player.PlayerMoney.CurrentRoundBet);
				player.PlayerMoney.ShouldPlayInRound = false;
				playerIndex++;

				await Task.Delay(1);
			}


			//BetController.MoveBet(null, RoundBets.Sum((x) => x.Action.Money), _tableViewModel);

			if (allPlayers.Count == 2)
			{
				// works only for heads-up
				ReturnMoneyInCaseOfAllIn();
			}
			else
			{
				ReturnMoneyInCaseUncalledBet();
			}
		}

		public async Task PlaceBlinds()
		{
			RoundBets.Clear();
			minRaise.Reset();

			var firstPlayer = allPlayers[allPlayers.Count == 2 ? 1 : initialPlayerIndex];
			var secondPlayer = allPlayers[allPlayers.Count == 2 ? 0 : initialPlayerIndex + 1];

			// Small blind
			PlaceBlindForPlayer(firstPlayer);

			await Task.Delay(1);

			// Big blind
			PlaceBlindForPlayer(secondPlayer);
		}

		public void PlaceBlindForPlayer(TDECORATOR player)
		{
			RoundBets.Add(
				new PlayerActionAndName(
					player.Name,
					player.PostingBlind(
						new PostingBlindContext(
							player.PlayerMoney.DoPlayerAction(PlayerAction.Post(Pot == 0 ? _smallBlind : _smallBlind * 2), 0),
							Pot - _smallBlind,
							player.PlayerMoney.Money))));

			//BetController.MoveBet(player.ChairView, player.PlayerMoney.CurrentRoundBet, player.ChairView);
		}

		private void ReturnMoneyInCaseOfAllIn()
		{
			var minMoneyPerPlayer = allPlayers.Min(x => x.PlayerMoney.CurrentRoundBet);
			foreach (var player in allPlayers)
			{
				player.PlayerMoney.NormalizeBets(minMoneyPerPlayer);
				player.ChairView?.SetMoneyView(player.PlayerMoney.Money);
			}
		}

		private void ReturnMoneyInCaseUncalledBet()
		{
			var group = allPlayers.GroupBy(x => x.PlayerMoney.CurrentRoundBet).OrderByDescending(k => k.Key);
			if (group.First().Count() == 1)
			{
				var player = group.First().First();
				player.PlayerMoney.NormalizeBets(group.ElementAt(1).First().PlayerMoney.CurrentRoundBet);
				player.ChairView?.SetMoneyView(player.PlayerMoney.Money);
			}
		}
	}
}
