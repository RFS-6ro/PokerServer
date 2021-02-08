using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Network;
using PokerSynchronisation;
using TexasHoldem.Logic.Cards;
using TexasHoldem.Logic.GameMechanics;
using TexasHoldem.Logic.Helpers;
using TexasHoldem.Logic.Players;

namespace PokerLobby
{
	public class DealerLogic<TDECORATOR>
		where TDECORATOR : PlayerDecorator, new()
	{
		private readonly int _handNumber;

		private readonly int _smallBlind;

		private readonly IList<TDECORATOR> _players;

		private readonly Deck _deck;

		private readonly List<Card> _communityCards;

		private readonly BettingLogicHandler<TDECORATOR> _bettingLogic;

		private Dictionary<string, ICollection<Card>> _showdownCards;

		private GameRoundType GameRoundType { get; set; } = GameRoundType.Posting;

		public DealerLogic(IList<TDECORATOR> players, int handNumber, int smallBlind)
		{
			_handNumber = handNumber;
			_smallBlind = smallBlind;
			_players = players;
			_deck = new Deck();
			_communityCards = new List<Card>(5);
			_bettingLogic = new BettingLogicHandler<TDECORATOR>(_players, smallBlind);
			_showdownCards = new Dictionary<string, ICollection<Card>>();
		}

		public async Task Play()
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



			if (GameRoundType == GameRoundType.Posting)
			{
				_bettingLogic.PlaceBlinds();
				GameRoundType = GameRoundType.Dispensing1;
			}

			if (GameRoundType == GameRoundType.Dispensing1)
			{
				//TODO: send second dispence event
				GameRoundType = GameRoundType.Dispensing2;
			}

			if (GameRoundType == GameRoundType.Dispensing2)
			{
				await Task.Delay(new Random().Next(100, 250));
				//TODO: send second dispence event
				GameRoundType = GameRoundType.PreFlop;
			}

			{
				//if (GameRoundType == GameRoundType.PreFlop)
				//{
				//	await PlayRound(GameRoundType);
				//}

				//if (GameRoundType == GameRoundType.Flop)
				//{
				//	await Task.Delay(new Random().Next(500));
				//	await PlayRound(GameRoundType, new int[] { 0, 1, 2 });
				//}

				//if (GameRoundType == GameRoundType.Turn)
				//{
				//	await Task.Delay(new Random().Next(500));
				//	await PlayRound(GameRoundType, new int[] { 3 });
				//}

				//if (GameRoundType == GameRoundType.River)
				//{
				//	await Task.Delay(new Random().Next(500));
				//	await PlayRound(GameRoundType, new int[] { 4 });
				//}
			}

			// Pre-flop -> blinds -> betting
			await PlayRound(GameRoundType.PreFlop);

			// Flop -> 3 cards -> betting
			if (_players.Count(x => x.PlayerMoney.InHand) > 1)
			{
				GameRoundType = GameRoundType.Flop;
				await Task.Delay(new Random().Next(500));
				await PlayRound(GameRoundType.Flop, new int[] { 0, 1, 2 });
			}

			// Turn -> 1 card -> betting
			if (_players.Count(x => x.PlayerMoney.InHand) > 1)
			{
				GameRoundType = GameRoundType.Turn;
				await Task.Delay(new Random().Next(500));
				await PlayRound(GameRoundType.Turn, new int[] { 3 });
			}

			// River -> 1 card -> betting
			if (_players.Count(x => x.PlayerMoney.InHand) > 1)
			{
				GameRoundType = GameRoundType.River;
				await Task.Delay(new Random().Next(500));
				await PlayRound(GameRoundType.River, new int[] { 4 });
			}

			GameRoundType = GameRoundType.Posting;

			DetermineWinnerAndAddPot(_bettingLogic.Pot, _bettingLogic.MainPot, _bettingLogic.SidePots);

			await Task.Delay(2000);

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

		private async Task PlayRound(GameRoundType gameRoundType, int[] communityCardsIndexes = null)
		{
			if (communityCardsIndexes != null)
			{
				for (var i = 0; i < communityCardsIndexes.Length; i++)
				{
					_communityCards.Add(_deck.GetNextCard());
				}
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

			int previousBank = _bettingLogic.Pot;

			await _bettingLogic.Bet(gameRoundType);

			if (_bettingLogic.Pot > previousBank)
			{
				//TODO: play sound and update bank
			}

			await Task.Delay(250);

			foreach (var player in _players)
			{
				var endRoundContext = new EndRoundContext(_bettingLogic.RoundBets);
				player.EndRound(endRoundContext);
			}
		}
	}
}
