namespace LobbyServer.pokerlogic.GameMechanics
{
	using LobbyServer.Client;
	using LobbyServer.Client.Handlers;
	using LobbyServer.pokerlogic.controllers;
	using LobbyServer.pokerlogic.pokermodel.Players;
	using LobbyServer.pokerlogic.pokermodel.UI;
	using PokerSynchronisation;
	using ServerDLL;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using UniCastCommonData;
	using UniCastCommonData.Network.MessageHandlers;
	using UniCastCommonData.Packet.InitialDatas;

	/*
	
		StaticLogger.Print($"Betting Logic + {Server.Id.ToString().Split('-')[0]}", $"new update pot event: pot = {0}");
		StaticLogger.Print($"Betting Logic + {Server.Id.ToString().Split('-')[0]}",
			new string[]
			{
				"multicasting for all users",
			}
		);

	 */
	public class BettingLogic
	{
		private readonly int initialPlayerIndex = 1;

		private readonly IList<ConsoleUiDecorator> allPlayers;

		private readonly int _smallBlind;

		private readonly TableViewModel _tableViewModel;

		private PotCreator potCreator;

		private MinRaise minRaise;

		private Lobby_Client_Server Server => Lobby_Client_Server.Instance;
		private SessionSender<Lobby_Client_Server> Sender => Lobby_Client_Server.Instance.SendHandler;

		public BettingLogic(IList<ConsoleUiDecorator> players, int smallBlind, TableViewModel tableViewModel)
		{
			//initialPlayerIndex = players.Count == 2 ? 0 : 1;
			allPlayers = players;
			_smallBlind = smallBlind;
			RoundBets = new List<PlayerActionAndName>();
			potCreator = new PotCreator(allPlayers);
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
				StaticLogger.Print($"Betting Logic + {Server.Id.ToString().Split('-')[0]}", $"pre flop situation");
				playerIndex = initialPlayerIndex + 2;
			}
			else
			{
				StaticLogger.Print($"Betting Logic + {Server.Id.ToString().Split('-')[0]}", $"clearing round bets");
				RoundBets.Clear();
				minRaise.Reset();
			}

			if (allPlayers.Count(x => x.PlayerMoney.ShouldPlayInRound) <= 1)
			{
				StaticLogger.Print($"Betting Logic + {Server.Id.ToString().Split('-')[0]}", $"not enough players to continue playing");
				return;
			}

			Dictionary<Guid, int> moneys = new();

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

				StaticLogger.Print($"Betting Logic + {Server.Id.ToString().Split('-')[0]}", $"new betting round. current turning player is {player.Name} with id {player.PlayerGuid}");



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
							5000,
							MainPot,
							SidePots);

				StaticLogger.Print($"Betting Logic + {Server.Id.ToString().Split('-')[0]}",
					new string[]
					{
						$"multicasting start turn for all users of round {gameRoundType}",
						$"turning player is {player.Name} with Id {player.PlayerGuid}, Money = {context.MoneyLeft}, currenyRoundBet = {context.MyMoneyInTheRound}",
						$"Max bet {maxMoneyPerPlayer}, Small blind {_smallBlind}, Pot {Pot}, MinRaise = {context.MinRaise}, MoneyToCall = {context.MoneyToCall}, CanRaise = {context.CanRaise}"
					}
				);

				Sender.Multicast(allPlayers.Select((x) => x.PlayerGuid),
								 new StartTurnSendingData(
									 player.PlayerGuid,
									 context.TimeForTurn,
									 (int)context.RoundType,
									 _smallBlind,
									 player.PlayerMoney.Money,
									 Pot,
									 player.PlayerMoney.CurrentRoundBet,
									 maxMoneyPerPlayer,
									 context.MinRaise,
									 context.MyMoneyInTheRound,
									 context.MoneyToCall,
									 context.IsAllIn,
									 context.CanRaise,
									 context.CanCheck,
									 Guid.Empty,
									 Server.Id,
									 Server.ServerType,
									 (int)lobbyTOclient.StartTurn),
								 null);

				//SEND player.ChairView.SetGameStateHolder(string.Empty);

				_tableViewModel.UpdateTableBeforeTurn(context);
				PlayerAction action = await player.AwaitTurn(context);

				StaticLogger.Print($"Betting Logic + {Server.Id.ToString().Split('-')[0]}", $"received action is {action}");
				action = player.PlayerMoney.DoPlayerAction(action, maxMoneyPerPlayer);
				StaticLogger.Print($"Betting Logic + {Server.Id.ToString().Split('-')[0]}", $"valid result action is {action}");

				maxMoneyPerPlayer = allPlayers.Max(x => x.PlayerMoney.CurrentRoundBet);

				StaticLogger.Print($"Betting Logic + {Server.Id.ToString().Split('-')[0]}", $"sending player turn to all players");
				Sender.Multicast(allPlayers.Select((x) => x.PlayerGuid),
								 new PlayerTurnSendingData(
									 player.PlayerGuid,
									 player.PlayerMoney.Money,
									 player.PlayerMoney.CurrentRoundBet,
									 -1,
									 action.Money,
									 minRaise.Amount(player.Name),
									 maxMoneyPerPlayer,
									 action.ToString(),
									 Guid.Empty,
									 Server.Id,
									 Server.ServerType,
									 (int)lobbyTOclient.PlayerTurn),
								 null);
				//SEND player.ChairView.SetGameStateHolder(action.ToString());

				StaticLogger.Print($"Betting Logic + {Server.Id.ToString().Split('-')[0]}", $"sending end turn of player {player.Name} with Id {player.PlayerGuid} to all players");
				Sender.Multicast(allPlayers.Select((x) => x.PlayerGuid),
								 new EndTurnSendingData(
									 player.PlayerGuid,
									 Guid.Empty,
									 Server.Id,
									 Server.ServerType,
									 (int)lobbyTOclient.EndTurn),
								 null);

				if (moneys.ContainsKey(player.PlayerGuid))
				{
					moneys[player.PlayerGuid] = player.PlayerMoney.Money;
				}
				else
				{
					moneys.Add(player.PlayerGuid, player.PlayerMoney.Money);
				}

				Sender.Multicast(allPlayers.Select((x) => x.PlayerGuid),
								 new UpdatePlayersMoneySendingData(
									 moneys,
									 Guid.Empty,
									 Server.Id,
									 Server.ServerType,
									 (int)lobbyTOclient.UpdatePlayersMoney),
								 null);
				//SEND player.ChairView?.SetMoneyView(player.PlayerMoney.Money);


				RoundBets.Add(new PlayerActionAndName(player.Name, action));

				//SEND BetController.MoveBet(player.ChairView, player.PlayerMoney.CurrentRoundBet, player.ChairView);

				if (action.Type == TurnType.Fold)
				{
					StaticLogger.Print($"Betting Logic + {Server.Id.ToString().Split('-')[0]}", $"players turns fold");
					Sender.Multicast(allPlayers.Select((x) => x.PlayerGuid),
									 new ClearCardsSendingData(
										 player.PlayerGuid,
										 Guid.Empty,
										 Server.Id,
										 Server.ServerType,
										 (int)lobbyTOclient.ClearCards),
									 null);
					//SEND player.ChairView.ClearCards();
				}

				if (action.Type == TurnType.Raise)
				{
					StaticLogger.Print($"Betting Logic + {Server.Id.ToString().Split('-')[0]}", $"players turns raise");
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


			StaticLogger.Print($"Betting Logic + {Server.Id.ToString().Split('-')[0]}", $"new update moneys event");
			Sender.Multicast(allPlayers.Select((x) => x.PlayerGuid),
							 new UpdatePlayersMoneySendingData(
								 moneys,
								 Guid.Empty,
								 Server.Id,
								 Server.ServerType,
								 (int)lobbyTOclient.UpdatePlayersMoney),
							 null);

			StaticLogger.Print($"Betting Logic + {Server.Id.ToString().Split('-')[0]}", $"new update pot event: pot = {0}");
			Sender.Multicast(allPlayers.Select((x) => x.PlayerGuid),
							 new UpdatePotSendingData(
								 Pot,
								 Guid.Empty,
								 Server.Id,
								 Server.ServerType,
								 (int)lobbyTOclient.UpdatepPot),
							 null);
			//SEND BetController.MoveBet(null, RoundBets.Sum((x) => x.Action.Money), _tableViewModel);

			if (allPlayers.Count == 2)
			{
				// works only for heads-up
				StaticLogger.Print($"Betting Logic + {Server.Id.ToString().Split('-')[0]}", $"Return Money In Case Of AllIn");
				ReturnMoneyInCaseOfAllIn();
			}
			else
			{
				StaticLogger.Print($"Betting Logic + {Server.Id.ToString().Split('-')[0]}", $"Return Money In Case Uncalled Bet");
				ReturnMoneyInCaseUncalledBet();
			}
		}

		public async Task PlaceBlinds()
		{
			RoundBets.Clear();
			minRaise.Reset();

			var firstPlayer = allPlayers[allPlayers.Count == 2 ? 1 : initialPlayerIndex];
			var secondPlayer = allPlayers[allPlayers.Count == 2 ? 0 : initialPlayerIndex + 1];

			StaticLogger.Print($"Betting Logic + {Server.Id.ToString().Split('-')[0]}",
				new string[]
				{
					$"sending blinds event. SmallBlind = {_smallBlind}",
					$"small blind player is {firstPlayer.Name} with Id {firstPlayer.PlayerGuid}, Money = {firstPlayer.PlayerMoney.Money}",
					$"big blind player is {secondPlayer.Name} with Id {secondPlayer.PlayerGuid}, Money = {secondPlayer.PlayerMoney.Money}",
				}
			);

			// Small blind
			PlaceBlindForPlayer(firstPlayer);

			await Task.Delay(1);

			// Big blind
			PlaceBlindForPlayer(secondPlayer);
		}

		public void PlaceBlindForPlayer(ConsoleUiDecorator player)
		{
			PlayerAction post = PlayerAction.Post(Pot == 0 ? _smallBlind : _smallBlind * 2);
			IPostingBlindContext context = new PostingBlindContext(
							player.PlayerMoney.DoPlayerAction(post, 0),
							Pot - _smallBlind,
							player.PlayerMoney.Money);

			_tableViewModel.UpdateTableBeforePostingBlind(context);
			RoundBets.Add(
				new PlayerActionAndName(
					player.Name,
					player.PostingBlind(context)));

			Sender.Multicast(allPlayers.Select((x) => x.PlayerGuid),
							 new PlayerTurnSendingData(
								 player.PlayerGuid,
								 player.PlayerMoney.Money,
								 0,
								 0,
								 post.Money,
								 _smallBlind * 2,
								 (Pot == 0 ? _smallBlind : _smallBlind * 2),
								 post.ToString(),
								 Guid.Empty,
								 Server.Id,
								 Server.ServerType,
								 (int)lobbyTOclient.PlayerTurn),
							 null);
			//SEND BetController.MoveBet(player.ChairView, player.PlayerMoney.CurrentRoundBet, player.ChairView);
		}

		private void ReturnMoneyInCaseOfAllIn()
		{
			Dictionary<Guid, int> moneys = new();
			var minMoneyPerPlayer = allPlayers.Min(x => x.PlayerMoney.CurrentRoundBet);
			foreach (var player in allPlayers)
			{
				player.PlayerMoney.NormalizeBets(minMoneyPerPlayer);
				moneys.Add(player.PlayerGuid, player.PlayerMoney.Money);
				//SEND player.ChairView?.SetMoneyView(player.PlayerMoney.Money);
			}

			Sender.Multicast(allPlayers.Select((x) => x.PlayerGuid),
							 new UpdatePlayersMoneySendingData(
								 moneys,
								 Guid.Empty,
								 Server.Id,
								 Server.ServerType,
								 (int)lobbyTOclient.UpdatePlayersMoney),
							 null);
		}

		private void ReturnMoneyInCaseUncalledBet()
		{
			Dictionary<Guid, int> moneys = new();
			var group = allPlayers.GroupBy(x => x.PlayerMoney.CurrentRoundBet).OrderByDescending(k => k.Key);
			if (group.First().Count() == 1)
			{
				var player = group.First().First();
				player.PlayerMoney.NormalizeBets(group.ElementAt(1).First().PlayerMoney.CurrentRoundBet);
				moneys.Add(player.PlayerGuid, player.PlayerMoney.Money);
				//SEND player.ChairView?.SetMoneyView(player.PlayerMoney.Money);
			}

			Sender.Multicast(allPlayers.Select((x) => x.PlayerGuid),
							 new UpdatePlayersMoneySendingData(
								 moneys,
								 Guid.Empty,
								 Server.Id,
								 Server.ServerType,
								 (int)lobbyTOclient.UpdatePlayersMoney),
							 null);
		}
	}
}
