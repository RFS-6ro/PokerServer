using LobbyServer.pokerlogic.controllers;
using LobbyServer.pokerlogic.pokermodel.Players;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TexasHoldem.Logic.Cards;
using TexasHoldem.Logic.GameMechanics;
using TexasHoldem.Logic.Helpers;
using TexasHoldem.Logic.Players;

namespace GameCore.Poker.Model
{
	public class HandLogic<TDECORATOR>
		where TDECORATOR : PlayerDecorator, new()
	{
		private readonly int _handNumber;

		private readonly int _smallBlind;

		private readonly IList<TDECORATOR> _players;

		private readonly Deck deck;

		private readonly List<Card> communityCards;

		private readonly BettingLogic<TDECORATOR> bettingLogic;

		private readonly TableViewModel _tableViewModel;

		private Dictionary<string, ICollection<Card>> showdownCards;

		public HandLogic(IList<TDECORATOR> players, int handNumber, int smallBlind, TableViewModel tableViewModel)
		{
			_handNumber = handNumber;
			_smallBlind = smallBlind;
			_players = players;
			deck = new Deck();
			communityCards = new List<Card>(5);
			bettingLogic = new BettingLogic<TDECORATOR>(players, smallBlind, tableViewModel);
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
				player.StartHand(startHandContext);
			}

			MoveDealerButton();

			await bettingLogic.PlaceBlinds();

			await DispencingCards();

			InitTableView();

			// Pre-flop -> blinds -> betting
			await PlayRound(PokerSynchronisation.GameRoundType.PreFlop, 0);

			// Flop -> 3 cards -> betting
			if (_players.Count(x => x.PlayerMoney.InHand) > 1)
			{
				ResetStates();
				await Task.Delay(500);
				await PlayRound(PokerSynchronisation.GameRoundType.Flop, 3);
			}

			// Turn -> 1 card -> betting
			if (_players.Count(x => x.PlayerMoney.InHand) > 1)
			{
				ResetStates();
				await Task.Delay(500);
				await PlayRound(PokerSynchronisation.GameRoundType.Turn, 1);
			}

			// River -> 1 card -> betting
			if (_players.Count(x => x.PlayerMoney.InHand) > 1)
			{
				ResetStates();
				await Task.Delay(500);
				await PlayRound(PokerSynchronisation.GameRoundType.River, 1);
			}

			ResetStates(true);

			DetermineWinnerAndAddPot(bettingLogic.Pot, bettingLogic.MainPot, bettingLogic.SidePots);

			_tableViewModel.ClearCards();

			foreach (var player in _players)
			{
				player.EndHand(new EndHandContext(showdownCards));
				player.ChairView.ClearCards();
			}

			await Task.Delay(5 * 1000);
		}

		private void ResetStates(bool all = false)
		{
			IEnumerable<TDECORATOR> players;
			if (all)
			{
				players = _players;
			}
			else
			{
				players = _players.Where(x => x.PlayerMoney.InHand);
			}

			foreach (var player in players)
			{
				player.ChairView.SetGameStateHolder(string.Empty);
			}
		}

		private async Task DispencingCards()
		{
			await DispenceCircle(0);

			await Task.Delay(1000);

			await DispenceCircle(1);
		}

		private async Task DispenceCircle(int index)
		{
			foreach (var player in _players)
			{
				Card card = deck.GetNextCard();

				//CardController.Dispence(player.ChairView, card, index);

				await Task.Delay(500);
			}
		}

		private void InitTableView()
		{
			//BetController.MoveBet(null, 0, _tableViewModel, 0f, false);
		}

		private void MoveDealerButton()
		{
			//_tableViewModel.MoveDealerButton(_players[0]);
		}

		private void DetermineWinnerAndAddPot(int pot, Pot mainPot, List<Pot> sidePot)
		{
			if (_players.Count(x => x.PlayerMoney.InHand) == 1)
			{
				var winner = _players.FirstOrDefault(x => x.PlayerMoney.InHand);
				winner.PlayerMoney.Money += pot;
				//BetController.MoveBet(_tableViewModel, pot, winner.ChairView);
				winner?.ChairView.ShowCards();
			}
			else
			{
				// showdown
				foreach (var player in _players)
				{
					if (player.PlayerMoney.InHand)
					{
						showdownCards.Add(player.Name, player.Cards);
					}
				}

				if (_players.Count == 2)
				{
					var betterHand = Helpers.CompareCards(
					_players[0].Cards.Concat(communityCards),
					_players[1].Cards.Concat(communityCards));
					if (betterHand > 0)
					{
						_players[0].PlayerMoney.Money += pot;
						//BetController.MoveBet(_tableViewModel, pot, _players[0].ChairView);
						_players[0]?.ChairView.ShowCards();
					}
					else if (betterHand < 0)
					{
						_players[1].PlayerMoney.Money += pot;
						//BetController.MoveBet(_tableViewModel, pot, _players[1].ChairView);
						_players[1]?.ChairView.ShowCards();
					}
					else
					{
						_players[0].PlayerMoney.Money += pot / 2;
						//BetController.MoveBet(_tableViewModel, pot / 2, _players[0].ChairView);
						_players[0]?.ChairView.ShowCards();
						_players[1].PlayerMoney.Money += pot / 2;
						//BetController.MoveBet(_tableViewModel, pot / 2, _players[1].ChairView);
						_players[1]?.ChairView.ShowCards();
					}
				}
				else
				{
					var handRankValueOfPlayers = new SortedDictionary<int, ICollection<string>>();
					var playersInHand = _players.Where(p => p.PlayerMoney.InHand);

					foreach (var player in playersInHand)
					{
						var opponents = playersInHand.Where(p => p.Name != player.Name).Select(s => s.Cards);
						var handRankValue = Helpers.GetHandRankValue(player.Cards, opponents, communityCards);

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
										player.PlayerMoney.Money += prize;
										//BetController.MoveBet(_tableViewModel, prize, player.ChairView);
										player?.ChairView.ShowCards();
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
		}

		private async Task PlayRound(PokerSynchronisation.GameRoundType gameRoundType, int communityCardsCount)
		{
			for (var i = 0; i < communityCardsCount; i++)
			{
				Card card = deck.GetNextCard();

				communityCards.Add(card);

				//CardController.Dispence(_tableViewModel, card, communityCards.Count - 1);
			}

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
			}

			await bettingLogic.Bet(gameRoundType);

			foreach (var player in _players)
			{
				var endRoundContext = new EndRoundContext(bettingLogic.RoundBets);
				player.EndRound(endRoundContext);
			}

			await Task.Delay(500);
		}
	}
}
