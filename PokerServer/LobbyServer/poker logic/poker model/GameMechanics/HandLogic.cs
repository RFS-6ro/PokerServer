using GameCore.Card.Poker;
using GameCore.Poker.Controller;
using GameCore.Poker.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TexasHoldem.Logic.Cards;
using TexasHoldem.Logic.GameMechanics;
using TexasHoldem.Logic.Helpers;
using TexasHoldem.Logic.Players;
using UnityEngine;

namespace GameCore.Poker.Model
{
	public class HandLogic<TDECORATOR>
		where TDECORATOR : UnityPlayerDecorator, new()
	{
		private readonly int _handNumber;

		private readonly int _smallBlind;

		private readonly IList<TDECORATOR> _players;

		private readonly Deck deck;

		private readonly List<CardData> communityCards;

		private readonly BettingLogic<TDECORATOR> bettingLogic;

		private readonly TableViewModel _tableViewModel;

		private Dictionary<string, ICollection<CardData>> showdownCards;

		public HandLogic(IList<TDECORATOR> players, int handNumber, int smallBlind, TableViewModel tableViewModel)
		{
			_handNumber = handNumber;
			_smallBlind = smallBlind;
			_players = players;
			deck = new Deck();
			communityCards = new List<CardData>(5);
			bettingLogic = new BettingLogic<TDECORATOR>(players, smallBlind, tableViewModel);
			showdownCards = new Dictionary<string, ICollection<CardData>>();
			_tableViewModel = tableViewModel;
		}

		public IEnumerator Play()
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

			yield return bettingLogic.PlaceBlinds();

			yield return DispencingCards();

			InitTableView();

			// Pre-flop -> blinds -> betting
			yield return PlayRound(PokerSynchronisation.GameRoundType.PreFlop, 0);

			// Flop -> 3 cards -> betting
			if (_players.Count(x => x.PlayerMoney.InHand) > 1)
			{
				ResetStates();
				yield return new WaitForSeconds(0.5f);
				yield return PlayRound(PokerSynchronisation.GameRoundType.Flop, 3);
			}

			// Turn -> 1 card -> betting
			if (_players.Count(x => x.PlayerMoney.InHand) > 1)
			{
				ResetStates();
				yield return new WaitForSeconds(0.5f);
				yield return PlayRound(PokerSynchronisation.GameRoundType.Turn, 1);
			}

			// River -> 1 card -> betting
			if (_players.Count(x => x.PlayerMoney.InHand) > 1)
			{
				ResetStates();
				yield return new WaitForSeconds(0.5f);
				yield return PlayRound(PokerSynchronisation.GameRoundType.River, 1);
			}

			ResetStates(true);

			DetermineWinnerAndAddPot(bettingLogic.Pot, bettingLogic.MainPot, bettingLogic.SidePots);

			_tableViewModel.ClearCards();

			foreach (var player in _players)
			{
				player.EndHand(new EndHandContext(showdownCards));
				player.ChairView.ClearCards();
			}

			yield return new WaitForSeconds(5f);
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

		private IEnumerator DispencingCards()
		{
			yield return DispenceCircle(0);

			yield return null;
			yield return new WaitForSeconds(1f);

			yield return DispenceCircle(1);
		}

		private IEnumerator DispenceCircle(int index)
		{
			foreach (var player in _players)
			{
				CardData card = deck.GetNextCard();
				Action<CardModel> onDispenceCompleted = player.AddCard(card, index == 0);

				CardController.Dispence(player.ChairView, card, index, 0.15f, onDispenceCompleted);

				yield return new WaitForSeconds(0.5f);
			}
		}

		private void InitTableView()
		{
			BetController.MoveBet(null, 0, _tableViewModel, 0f, false);
		}

		private void MoveDealerButton()
		{
			_tableViewModel.DealerButton.Move(_players[0].ChairView.DealerPosition.position, 0f);
		}

		private void DetermineWinnerAndAddPot(int pot, TexasHoldem.Logic.GameMechanics.Pot mainPot, List<TexasHoldem.Logic.GameMechanics.Pot> sidePot)
		{
			if (_players.Count(x => x.PlayerMoney.InHand) == 1)
			{
				var winner = _players.FirstOrDefault(x => x.PlayerMoney.InHand);
				winner.PlayerMoney.Money += pot;
				BetController.MoveBet(_tableViewModel, pot, winner.ChairView, 0f, true);
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
						BetController.MoveBet(_tableViewModel, pot, _players[0].ChairView, 0f, true);
						_players[0]?.ChairView.ShowCards();
					}
					else if (betterHand < 0)
					{
						_players[1].PlayerMoney.Money += pot;
						BetController.MoveBet(_tableViewModel, pot, _players[1].ChairView, 0f, true);
						_players[1]?.ChairView.ShowCards();
					}
					else
					{
						_players[0].PlayerMoney.Money += pot / 2;
						BetController.MoveBet(_tableViewModel, pot / 2, _players[0].ChairView, 0f, true);
						_players[0]?.ChairView.ShowCards();
						_players[1].PlayerMoney.Money += pot / 2;
						BetController.MoveBet(_tableViewModel, pot / 2, _players[1].ChairView, 0f, true);
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

					var remainingPots = new Stack<TexasHoldem.Logic.GameMechanics.Pot>();
					var pots = new Stack<TexasHoldem.Logic.GameMechanics.Pot>(sidePot);
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
										BetController.MoveBet(_tableViewModel, prize, player.ChairView, 0f, true);
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

		private IEnumerator PlayRound(PokerSynchronisation.GameRoundType gameRoundType, int communityCardsCount)
		{
			for (var i = 0; i < communityCardsCount; i++)
			{
				CardData card = deck.GetNextCard();

				communityCards.Add(card);

				CardController.Dispence(_tableViewModel, card, communityCards.Count - 1, 0.15f, (x) => x.Show());
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

			yield return bettingLogic.Bet(gameRoundType);

			foreach (var player in _players)
			{
				var endRoundContext = new EndRoundContext(bettingLogic.RoundBets);
				player.EndRound(endRoundContext);
			}

			yield return new WaitForSeconds(0.5f);
		}
	}
}
