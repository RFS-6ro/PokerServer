using System;
using System.Collections.Generic;
using System.Linq;
using PokerSynchronisation;
using TexasHoldem.Logic.Cards;
using TexasHoldem.Logic.GameMechanics;
using TexasHoldem.Logic.Helpers;
using TexasHoldem.Logic.Players;

namespace PokerLobby
{
	public class DealerLogic : IHandLogic
	{
		private readonly int _handNumber;

		private readonly int _smallBlind;

		private readonly IList<InternalPlayer> _players;

		private readonly Deck _deck;

		private readonly List<Card> _communityCards;

		private readonly BettingHandler _bettingLogic;

		private Dictionary<string, ICollection<Card>> _showdownCards;

		public DealerLogic(IList<InternalPlayer> players, int handNumber, int smallBlind)
		{
			_handNumber = handNumber;
			_smallBlind = smallBlind;
			_players = players;
			_deck = new Deck();
			_communityCards = new List<Card>(5);
			_bettingLogic = new BettingHandler(_players, smallBlind);
			_showdownCards = new Dictionary<string, ICollection<Card>>();
		}

		public void Play()
		{
			// Start the hand and deal cards to each player
			foreach (var player in _players)
			{
				var startHandContext = new StartHandContext(
					_deck.GetNextCard(),
					_deck.GetNextCard(),
					_handNumber,
					player.PlayerMoney.Money,
					_smallBlind,
					_players[0].Name);
				player.StartHand(startHandContext);
			}

			// Pre-flop -> blinds -> betting
			PlayRound(GameRoundType.PreFlop, 0);

			// Flop -> 3 cards -> betting
			if (_players.Count(x => x.PlayerMoney.InHand) > 1)
			{
				PlayRound(GameRoundType.Flop, 3);
			}

			// Turn -> 1 card -> betting
			if (_players.Count(x => x.PlayerMoney.InHand) > 1)
			{
				PlayRound(GameRoundType.Turn, 1);
			}

			// River -> 1 card -> betting
			if (_players.Count(x => x.PlayerMoney.InHand) > 1)
			{
				PlayRound(GameRoundType.River, 1);
			}

			DetermineWinnerAndAddPot(_bettingLogic.Pot, _bettingLogic.MainPot, _bettingLogic.SidePots);

			foreach (var player in _players)
			{
				player.EndHand(new EndHandContext(_showdownCards));
			}
		}

		private void DetermineWinnerAndAddPot(int pot, Pot mainPot, List<Pot> sidePot)
		{
			if (_players.Count(x => x.PlayerMoney.InHand) == 1)
			{
				var winner = _players.FirstOrDefault(x => x.PlayerMoney.InHand);
				winner.PlayerMoney.Money += pot;
			}
			else
			{
				// showdown
				foreach (var player in _players)
				{
					if (player.PlayerMoney.InHand)
					{
						_showdownCards.Add(player.Name, player.Cards);
					}
				}

				if (_players.Count == 2)
				{
					var betterHand = Helpers.CompareCards(
					_players[0].Cards.Concat(_communityCards),
					_players[1].Cards.Concat(_communityCards));
					if (betterHand > 0)
					{
						_players[0].PlayerMoney.Money += pot;
					}
					else if (betterHand < 0)
					{
						_players[1].PlayerMoney.Money += pot;
					}
					else
					{
						_players[0].PlayerMoney.Money += pot / 2;
						_players[1].PlayerMoney.Money += pot / 2;
					}
				}
				else
				{
					var handRankValueOfPlayers = new SortedDictionary<int, ICollection<string>>();
					var playersInHand = _players.Where(p => p.PlayerMoney.InHand);

					foreach (var player in playersInHand)
					{
						var opponents = playersInHand.Where(p => p.Name != player.Name).Select(s => s.Cards);
						var handRankValue = Helpers.GetHandRankValue(player.Cards, opponents, _communityCards);

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
										_players.First(x => x.Name == nominee).PlayerMoney.Money += prize;
									}
								}
								else
								{
									// There were no active players with the current strength of the hands taking pot
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

		private void PlayRound(GameRoundType gameRoundType, int communityCardsCount)
		{
			for (var i = 0; i < communityCardsCount; i++)
			{
				_communityCards.Add(_deck.GetNextCard());
			}

			foreach (var player in _players)
			{
				var startRoundContext = new StartRoundContext(
					gameRoundType,
					_communityCards.AsReadOnly(),
					player.PlayerMoney.Money,
					_bettingLogic.Pot,
					_bettingLogic.MainPot,
					_bettingLogic.SidePots);
				player.StartRound(startRoundContext);
			}

			_bettingLogic.Bet(gameRoundType);

			foreach (var player in _players)
			{
				var endRoundContext = new EndRoundContext(_bettingLogic.RoundBets);
				player.EndRound(endRoundContext);
			}
		}
	}
}
