using LobbyServer.Client;
using LobbyServer.Client.Handlers;
using LobbyServer.pokerlogic.controllers;
using LobbyServer.pokerlogic.pokermodel.Players;
using LobbyServer.pokerlogic.pokermodel.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LobbyServer.pokerlogic.Cards;
using UniCastCommonData.Network.MessageHandlers;
using UniCastCommonData.Packet.InitialDatas;
using ServerDLL;

namespace LobbyServer.pokerlogic.GameMechanics
{
	/*

		StaticLogger.Print($"Hand Logic + {Server.Id.ToString().Split('-')[0]}", "");
		StaticLogger.Print($"Hand Logic + {Server.Id.ToString().Split('-')[0]}",
			new string[]
			{
				"multicasting for all users",
				text
			}
		);

	 */
	public class HandLogic
	{
		private readonly int _handNumber;

		private readonly int _smallBlind;

		private readonly IList<ConsoleUiDecorator> _players;

		private readonly Deck deck;

		private readonly List<Card> communityCards;

		private readonly BettingLogic bettingLogic;

		private readonly TableViewModel _tableViewModel;

		private Dictionary<string, ICollection<Card>> showdownCards;

		private Lobby_Client_Server Server => Lobby_Client_Server.Instance;
		private SessionSender<Lobby_Client_Server> Sender => Lobby_Client_Server.Instance.SendHandler;

		public HandLogic(IList<ConsoleUiDecorator> players, int handNumber, int smallBlind, TableViewModel tableViewModel)
		{
			_handNumber = handNumber;
			_smallBlind = smallBlind;
			_players = players;
			deck = new Deck();
			communityCards = new List<Card>(5);
			bettingLogic = new BettingLogic(players, smallBlind, tableViewModel);
			showdownCards = new Dictionary<string, ICollection<Card>>();
			_tableViewModel = tableViewModel;
		}

		public async Task Play()
		{
			// Start the hand and deal cards to each player
			foreach (var player in _players)
			{
				var startHandContext = new StartHandContext(
					_handNumber,
					player.PlayerMoney.Money,
					_smallBlind,
					_players[0].Name);

				_tableViewModel.StartHand();
				player.StartHand(startHandContext);

				Sender.SendAsync(new StartHandSendingData(
									 _handNumber,
									 player.PlayerMoney.Money,
									 _smallBlind,
									 _players[0].Name,
									 player.PlayerGuid,
									 Server.Id,
									 Server.ServerType,
									 (int)lobbyTOclient.StartHand),
								 null);
			}

			StaticLogger.Print($"Hand Logic + {Server.Id.ToString().Split('-')[0]}",
				new string[]
				{
					$"sending start hand event to this players ({_players.Count})",
				}
				.Concat(GetPlayersForLogs())
			);

			StaticLogger.Print($"Hand Logic + {Server.Id.ToString().Split('-')[0]}", "MoveDealerButton");
			MoveDealerButton();

			StaticLogger.Print($"Hand Logic + {Server.Id.ToString().Split('-')[0]}", "PlaceBlinds");
			await bettingLogic.PlaceBlinds();

			StaticLogger.Print($"Hand Logic + {Server.Id.ToString().Split('-')[0]}", "DispencingCards");
			await DispencingCards();

			StaticLogger.Print($"Hand Logic + {Server.Id.ToString().Split('-')[0]}", "InitTableView");
			InitTableView();

			StaticLogger.Print($"Hand Logic + {Server.Id.ToString().Split('-')[0]}",
				new string[]
				{
					"PreFlop"
				}
				.Concat(GetPlayersForLogs(_players, (player) => $" Is in hand = {player.PlayerMoney.InHand}"))
			);
			// Pre-flop -> blinds -> betting
			await PlayRound(PokerSynchronisation.GameRoundType.PreFlop, 0);

			// Flop -> 3 cards -> betting
			if (_players.Count(x => x.PlayerMoney.InHand) > 1)
			{
				StaticLogger.Print($"Hand Logic + {Server.Id.ToString().Split('-')[0]}",
					new string[]
					{
						"Flop"
					}
					.Concat(GetPlayersForLogs(_players, (player) => $" Is in hand = {player.PlayerMoney.InHand}"))
				);
				ResetStates();
				await Task.Delay(500);
				await PlayRound(PokerSynchronisation.GameRoundType.Flop, 3);
			}

			// Turn -> 1 card -> betting
			if (_players.Count(x => x.PlayerMoney.InHand) > 1)
			{
				StaticLogger.Print($"Hand Logic + {Server.Id.ToString().Split('-')[0]}",
					new string[]
					{
						"Turn"
					}
					.Concat(GetPlayersForLogs(_players, (player) => $" Is in hand = {player.PlayerMoney.InHand}"))
				);
				ResetStates();
				await Task.Delay(500);
				await PlayRound(PokerSynchronisation.GameRoundType.Turn, 1);
			}

			// River -> 1 card -> betting
			if (_players.Count(x => x.PlayerMoney.InHand) > 1)
			{
				StaticLogger.Print($"Hand Logic + {Server.Id.ToString().Split('-')[0]}",
					new string[]
					{
						"River"
					}
					.Concat(GetPlayersForLogs(_players, (player) => $" Is in hand = {player.PlayerMoney.InHand}"))
				);
				ResetStates();
				await Task.Delay(500);
				await PlayRound(PokerSynchronisation.GameRoundType.River, 1);
			}

			ResetStates(true);

			StaticLogger.Print($"HandLogic + {Server.Id.ToString().Split('-')[0]}", "Determining winners");
			DetermineWinnerAndAddPot(bettingLogic.Pot, bettingLogic.MainPot, bettingLogic.SidePots);

			await Task.Delay(5000);

			_tableViewModel.ClearCards();

			StaticLogger.Print($"HandLogic + {Server.Id.ToString().Split('-')[0]}", "sending event to clear all cards");
			Sender.Multicast(_players.Select((x) => x.PlayerGuid),
							 new ClearCardsSendingData(
								 Guid.Empty,
								 Guid.Empty,
								 Server.Id,
								 Server.ServerType,
								 (int)lobbyTOclient.ClearCards),
							 null);

			foreach (var player in _players)
			{
				player.EndHand(new EndHandContext(showdownCards));

				Sender.SendAsync(new EndHandSendingData(
									 player.PlayerGuid,
									 Server.Id,
									 Server.ServerType,
									 (int)lobbyTOclient.EndHand),
								 null);
				//SEND player.ChairView.ClearCards();
			}

			StaticLogger.Print($"Hand Logic + {Server.Id.ToString().Split('-')[0]}",
				new string[]
				{
					$"sending end hand event to this players ({_players.Count})",
				}
				.Concat(GetPlayersForLogs())
			);

			await Task.Delay(1000);
		}

		private void ResetStates(bool all = false)
		{
			IEnumerable<ConsoleUiDecorator> players;
			if (all)
			{
				players = _players;
			}
			else
			{
				players = _players.Where(x => x.PlayerMoney.InHand);
			}

			StaticLogger.Print($"Hand Logic + {Server.Id.ToString().Split('-')[0]}",
				new string[]
				{
					$"sending event to reset states to this players ({_players.Count})",
				}
				.Concat(GetPlayersForLogs(players))
			);

			foreach (var player in players)
			{
				Sender.Multicast(_players.Select((x) => x.PlayerGuid),
								 new PlayerTurnSendingData(
									 player.PlayerGuid,
									 player.PlayerMoney.Money,
									 -1,
									 -1,
									 -1,
									 _smallBlind * 2,
									 0,
									 string.Empty,
									 Guid.Empty,
									 Server.Id,
									 Server.ServerType,
									 (int)lobbyTOclient.PlayerTurn),
								 null);
				//SEND player.ChairView.SetGameStateHolder(string.Empty);
			}
		}

		private async Task DispencingCards()
		{
			DispenceCircle(0);

			DispenceCircle(1);

			foreach (var player in _players)
			{
				Sender.SendAsync(new DealCardsToPlayerSendingData(
									 (int)player.Cards[0].Type,
									 (int)player.Cards[0].Suit,
									 (int)player.Cards[1].Type,
									 (int)player.Cards[1].Suit,
									 player.PlayerGuid,
									 Server.Id,
									 Server.ServerType,
									 (int)lobbyTOclient.DealCardsToPlayer),
								 null);
			}

			StaticLogger.Print($"Hand Logic + {Server.Id.ToString().Split('-')[0]}",
				new string[]
				{
					$"sending event to deal cards to this players ({_players.Count})",
				}
				.Concat(GetPlayersForLogs(_players, (player) => $"[{player.Cards[0].Type}, {player.Cards[0].Suit}] [{player.Cards[1].Type}, {player.Cards[1].Suit}]"))
			);

			await Task.Delay(1000);
		}

		private void DispenceCircle(int index)
		{
			foreach (var player in _players)
			{
				Card card = deck.GetNextCard();
				player.AddCard(card, index == 0);
				//SEND CardController.Dispence(player.ChairView, card, index);
				//await Task.Delay(500);
			}
		}

		private void InitTableView()
		{
			StaticLogger.Print($"Hand Logic + {Server.Id.ToString().Split('-')[0]}", $"new update pot event: pot = {0}");
			Sender.Multicast(_players.Select((x) => x.PlayerGuid),
							 new UpdatePotSendingData(
								 0,
								 Guid.Empty,
								 Server.Id,
								 Server.ServerType,
								 (int)lobbyTOclient.UpdatepPot),
							 null);
			//SEND BetController.MoveBet(null, 0, _tableViewModel, 0f, false);
		}

		private void MoveDealerButton()
		{
			StaticLogger.Print($"Hand Logic + {Server.Id.ToString().Split('-')[0]}", $"new dealer id is {_players[0].PlayerGuid.ToString().Split('-')[0]}");
			Sender.Multicast(_players.Select((x) => x.PlayerGuid),
							 new DealerButtonSendingData(
								 _players[0].PlayerGuid,
								 Guid.Empty,
								 Server.Id,
								 Server.ServerType,
								 (int)lobbyTOclient.DealerButton),
							 null);
			//SEND _tableViewModel.MoveDealerButton(_players[0]);
		}

		private void DetermineWinnerAndAddPot(int pot, Pot mainPot, List<Pot> sidePot)
		{
			Dictionary<Guid, int> moneys = new();
			Dictionary<Guid, UniCastCommonData.Packet.InitialDatas.Tuple<int, int, int, int>> cards = new();
			List<UniCastCommonData.Packet.InitialDatas.Tuple<Guid, int, string>> winners = new();

			List<string> winnerLogs = new List<string>();
			winnerLogs.Add($"Pot = {pot}");

			if (_players.Count(x => x.PlayerMoney.InHand) == 1)
			{
				var winner = _players.FirstOrDefault(x => x.PlayerMoney.InHand);
				winner.PlayerMoney.Money += pot;
				cards.Add(winner.PlayerGuid,
						 new UniCastCommonData.Packet.InitialDatas.Tuple<int, int, int, int>
						 (
							(int)winner.Cards[0].Type,
							(int)winner.Cards[0].Suit,
							(int)winner.Cards[1].Type,
							(int)winner.Cards[1].Suit
						 )
				);
				//SEND BetController.MoveBet(_tableViewModel, pot, winner.ChairView);
				moneys.Add(winner.PlayerGuid, winner.PlayerMoney.Money);
				winners.Add(ConfigureWinner(winner, pot));
				winnerLogs.Add($"Player with ID {winner.PlayerGuid}, with money left: {winner.PlayerMoney.Money}, handType = {winners[winners.Count - 1].Item3}, Prize = {pot}, Card1 = [{winner.Cards[0].Type}, {winner.Cards[0].Suit}], Card2 = [{winner.Cards[1].Type}, {winner.Cards[1].Suit}]");
				//SEND winner?.ChairView.ShowCards();
			}
			else
			{
				// showdown
				foreach (var player in _players)
				{
					if (player.PlayerMoney.InHand)
					{
						cards.Add(player.PlayerGuid,
						 new UniCastCommonData.Packet.InitialDatas.Tuple<int, int, int, int>
								 (
									(int)player.Cards[0].Type,
									(int)player.Cards[0].Suit,
									(int)player.Cards[1].Type,
									(int)player.Cards[1].Suit
								 ));
						showdownCards.Add(player.Name, player.Cards);
					}
				}

				if (_players.Count == 2)
				{
					var betterHand = Helpers.Helpers.CompareCards(
					_players[0].Cards.Concat(communityCards),
					_players[1].Cards.Concat(communityCards));
					if (betterHand > 0)
					{
						_players[0].PlayerMoney.Money += pot;
						//SEND BetController.MoveBet(_tableViewModel, pot, _players[0].ChairView);
						moneys.Add(_players[0].PlayerGuid, _players[0].PlayerMoney.Money);
						winners.Add(ConfigureWinner(_players[0], pot));
						winnerLogs.Add($"Player with ID {_players[0].PlayerGuid}, with money left: {_players[0].PlayerMoney.Money}, handType = {winners[winners.Count - 1].Item3}, Prize = {pot}, Card1 = [{_players[0].Cards[0].Type}, {_players[0].Cards[0].Suit}], Card2 = [{_players[0].Cards[1].Type}, {_players[0].Cards[1].Suit}]");
						//SEND _players[0]?.ChairView.ShowCards();
					}
					else if (betterHand < 0)
					{
						_players[1].PlayerMoney.Money += pot;
						//SEND BetController.MoveBet(_tableViewModel, pot, _players[1].ChairView);
						moneys.Add(_players[1].PlayerGuid, _players[1].PlayerMoney.Money);
						winners.Add(ConfigureWinner(_players[1], pot));
						winnerLogs.Add($"Player with ID {_players[1].PlayerGuid}, with money left: {_players[1].PlayerMoney.Money}, handType = {winners[winners.Count - 1].Item3}, Prize = {pot}, Card1 = [{_players[1].Cards[0].Type}, {_players[1].Cards[0].Suit}], Card2 = [{_players[1].Cards[1].Type}, {_players[1].Cards[1].Suit}]");
						//SEND _players[1]?.ChairView.ShowCards();
					}
					else
					{
						_players[0].PlayerMoney.Money += pot / 2;
						//SEND BetController.MoveBet(_tableViewModel, pot / 2, _players[0].ChairView);
						moneys.Add(_players[0].PlayerGuid, _players[0].PlayerMoney.Money);
						winners.Add(ConfigureWinner(_players[0], pot / 2));
						winnerLogs.Add($"Player with ID {_players[0].PlayerGuid}, with money left: {_players[0].PlayerMoney.Money}, handType = {winners[winners.Count - 1].Item3}, Prize = {pot / 2}, Card1 = [{_players[0].Cards[0].Type}, {_players[0].Cards[0].Suit}], Card2 = [{_players[0].Cards[1].Type}, {_players[0].Cards[1].Suit}]");
						//SEND _players[0]?.ChairView.ShowCards();


						_players[1].PlayerMoney.Money += pot / 2;
						//SEND BetController.MoveBet(_tableViewModel, pot / 2, _players[1].ChairView);
						moneys.Add(_players[1].PlayerGuid, _players[1].PlayerMoney.Money);
						winners.Add(ConfigureWinner(_players[1], pot / 2));
						winnerLogs.Add($"Player with ID {_players[1].PlayerGuid}, with money left: {_players[1].PlayerMoney.Money}, handType = {winners[winners.Count - 1].Item3}, Prize = {pot / 2}, Card1 = [{_players[1].Cards[0].Type}, {_players[1].Cards[0].Suit}], Card2 = [{_players[1].Cards[1].Type}, {_players[1].Cards[1].Suit}]");
						//SEND _players[1]?.ChairView.ShowCards();
					}
				}
				else
				{
					var handRankValueOfPlayers = new SortedDictionary<int, ICollection<string>>();
					var playersInHand = _players.Where(p => p.PlayerMoney.InHand);

					foreach (var player in playersInHand)
					{
						var opponents = playersInHand.Where(p => p.Name != player.Name).Select(s => s.Cards);
						var handRankValue = Helpers.Helpers.GetHandRankValue(player.Cards, opponents, communityCards);

						if (handRankValueOfPlayers.ContainsKey(handRankValue))
						{
							handRankValueOfPlayers[handRankValue].Add(player.Name);
						}
						else
						{
							handRankValueOfPlayers.Add(handRankValue, new List<string> { player.Name });
						}
					}

					var remainingPots = new Stack<Pot>();
					var pots = new Stack<Pot>(sidePot);
					pots.Push(mainPot);

					foreach (var playersWithTheBestHand in handRankValueOfPlayers.Reverse())
					{
						do
						{
							var oneOfThePots = pots.Pop();

							if (oneOfThePots.ActivePlayer.Count == 0)
							{
								throw new Exception("There are no players in the pot");
							}
							else if (oneOfThePots.ActivePlayer.Count == 1)
							{
								continue;
							}
							else
							{
								var nominees = oneOfThePots.ActivePlayer.Intersect(playersWithTheBestHand.Value);
								var count = nominees.Count();

								if (count > 0)
								{
									var prize = oneOfThePots.AmountOfMoney / count; // TODO: If there are odd chips in a split pot.

									foreach (var nominee in nominees)
									{
										var player = _players.First(x => x.Name == nominee);
										moneys.Add(player.PlayerGuid, player.PlayerMoney.Money);
										winners.Add(ConfigureWinner(player, prize));
										winnerLogs.Add($"Player with ID {player.PlayerGuid}, with money left: {player.PlayerMoney.Money}, handType = {winners[winners.Count - 1].Item3}, Prize = {prize}, Card1 = [{player.Cards[0].Type}, {player.Cards[0].Suit}], Card2 = [{player.Cards[1].Type}, {player.Cards[1].Suit}]");
									}
								}
								else
								{
									// There were no active players with the current strength of the hands taking this pot
									remainingPots.Push(oneOfThePots);
								}
							}
						}
						while (pots.Count > 0);

						if (remainingPots.Count == 0)
						{
							break;
						}
						else
						{
							while (remainingPots.Count > 0)
							{
								pots.Push(remainingPots.Pop());
							}
						}
					}
				}
			}

			StaticLogger.Print($"Hand Logic + {Server.Id.ToString().Split('-')[0]}",
				new string[]
				{
					"sending winner event with cards and player money"
				}.Concat(winnerLogs)
			);

			Sender.Multicast(_players.Select((x) => x.PlayerGuid),
							 new WinnersSendingData(
								 winners,
								 Guid.Empty,
								 Server.Id,
								 Server.ServerType,
								 (int)lobbyTOclient.Winners),
							 null);

			Sender.Multicast(_players.Select((x) => x.PlayerGuid),
							 new OpponentCardsSendingData(
								 cards,
								 Guid.Empty,
								 Server.Id,
								 Server.ServerType,
								 (int)lobbyTOclient.OpponentCards),
							 null);


			Sender.Multicast(_players.Select((x) => x.PlayerGuid),
							 new UpdatePlayersMoneySendingData(
								 moneys,
								 Guid.Empty,
								 Server.Id,
								 Server.ServerType,
								 (int)lobbyTOclient.UpdatePlayersMoney),
							 null);
		}

		private UniCastCommonData.Packet.InitialDatas.Tuple<Guid, int, string> ConfigureWinner(ConsoleUiDecorator player, int prize)
		{
			player.PlayerMoney.Money += prize;
			//SEND BetController.MoveBet(_tableViewModel, prize, player.ChairView);
			string handType = Helpers.Helpers.GetHandRank(player.Cards.Concat(communityCards).ToList()).ToString();
			string handString = handType + new string(' ', 13 - handType.Length);
			player.SetWinner(prize, handType);

			return new UniCastCommonData.Packet.InitialDatas.Tuple<Guid, int, string>(player.PlayerGuid, prize, handString);
		}

		private async Task PlayRound(PokerSynchronisation.GameRoundType gameRoundType, int communityCardsCount)
		{
			List<UniCastCommonData.Packet.InitialDatas.Tuple<int, int, int>> cards = new();
			int index = communityCards.Count;
			List<string> cardLogs = new List<string>();
			for (var i = 0; i < communityCardsCount; i++)
			{
				Card card = deck.GetNextCard();

				communityCards.Add(card);

				cardLogs.Add($"[{card.Type}, {card.Suit}] on index {index}");
				cards.Add(new UniCastCommonData.Packet.InitialDatas.Tuple<int, int, int>((int)card.Type, (int)card.Suit, index));
				index++;
				//SEND CardController.Dispence(_tableViewModel, card, communityCards.Count - 1);
			}

			StaticLogger.Print($"Hand Logic + {Server.Id.ToString().Split('-')[0]}",
				new string[]
				{
					"sending table cards data for every player"
				}
				.Concat(cardLogs)
			);

			Sender.Multicast(_players.Select((x) => x.PlayerGuid),
							 new DealCardsToTableSendingData(
								 cards,
								 Guid.Empty,
								 Server.Id,
								 Server.ServerType,
								 (int)lobbyTOclient.DealCardsToTable),
							 null);

			StaticLogger.Print($"Hand Logic + {Server.Id.ToString().Split('-')[0]}", $"Table start round, pot = {bettingLogic.Pot}");
			_tableViewModel.StartRound
			(
				communityCards.AsReadOnly(),
				bettingLogic.Pot,
				bettingLogic.MainPot.AmountOfMoney,
				bettingLogic.SidePots.Select((x) => x.AmountOfMoney)
			);

			foreach (var player in _players)
			{
				var startRoundContext = new StartRoundContext(
					gameRoundType,
					communityCards.AsReadOnly(),
					player.PlayerMoney.Money,
					bettingLogic.Pot,
					bettingLogic.MainPot,
					bettingLogic.SidePots);

				player.StartRound(startRoundContext);

				Sender.SendAsync(new StartRoundSendingData(
									 bettingLogic.Pot,
									 player.PlayerMoney.Money,
									 player.PlayerGuid,
									 Server.Id,
									 Server.ServerType,
									 (int)lobbyTOclient.StartRound),
								 null);
			}

			StaticLogger.Print($"Hand Logic + {Server.Id.ToString().Split('-')[0]}",
				new string[]
				{
					$"sending end hand event to this players ({_players.Count})",
				}
				.Concat(GetPlayersForLogs())
			);

			StaticLogger.Print($"Hand Logic + {Server.Id.ToString().Split('-')[0]}", "awaiting for betting round");
			await bettingLogic.Bet(gameRoundType);

			foreach (var player in _players)
			{
				var endRoundContext = new EndRoundContext(bettingLogic.RoundBets);
				player.EndRound(endRoundContext);
				Sender.SendAsync(new EndRoundSendingData(
									 player.PlayerGuid,
									 Server.Id,
									 Server.ServerType,
									 (int)lobbyTOclient.EndRound),
								 null);
			}

			StaticLogger.Print($"Hand Logic + {Server.Id.ToString().Split('-')[0]}",
				new string[]
				{
					$"sending end round event to this players ({_players.Count})",
				}
				.Concat(GetPlayersForLogs(_players, (player) => $" Is in hand = {player.PlayerMoney.InHand}"))
			);

			await Task.Delay(500);
		}

		internal IEnumerable<string> GetPlayersForLogs(IEnumerable<ConsoleUiDecorator> players = null, Func<ConsoleUiDecorator, string> getAdditionalInfo = null)
		{
			if (players == null)
			{
				players = _players;
			}
			List<string> logPlayers = new List<string>();

			int index = 0;
			foreach (var player in players)
			{
				string additionalInfo = getAdditionalInfo?.Invoke(player);
				logPlayers.Add($"{index}) {player.Name} {player.PlayerGuid}" + additionalInfo ?? string.Empty);
				index++;
			}

			return logPlayers;
		}
	}
}
