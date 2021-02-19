using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Network;
using PokerSynchronisation;
using TexasHoldem.Logic.Cards;
using TexasHoldem.Logic.GameMechanics;
using TexasHoldem.Logic.Helpers;
using TexasHoldem.Logic.Players;

namespace PokerLobby
{
	public class DealerLogic
	{
		private readonly int _handNumber;

		private readonly int _smallBlind;

		private readonly IList<RealPlayerDecorator> _players;

		private readonly Deck _deck;

		private readonly List<Card> _communityCards;

		private readonly BettingLogicHandler _bettingLogic;

		private Dictionary<string, ICollection<Card>> _showdownCards;

		private GameRoundType GameRoundType { get; set; } = GameRoundType.Posting;

		public DealerLogic(IList<RealPlayerDecorator> players, int handNumber, int smallBlind)
		{
			_handNumber = handNumber;
			_smallBlind = smallBlind;
			_players = players;
			_deck = new Deck();
			_communityCards = new List<Card>(5);
			_bettingLogic = new BettingLogicHandler(_players, smallBlind);
			_showdownCards = new Dictionary<string, ICollection<Card>>();
		}

		public async Task Play()
		{
			// Start the hand and deal cards to each player
			List<Card> dispence1 = new List<Card>();
			List<Card> dispence2 = new List<Card>();
			List<int> playerIds = new List<int>();

			foreach (var player in _players)
			{
				Card card1 = _deck.GetNextCard();
				dispence1.Add(card1);
				Card card2 = _deck.GetNextCard();
				dispence2.Add(card2);
				var startHandContext = new StartHandContext(
					card1,
					card2,
					_handNumber,
					player.PlayerMoney.Money,
					_smallBlind,
					_players[0].Name);
				player.StartHand(startHandContext);
				playerIds.Add(player.ServerId);
			}


			//Blinds
			GameRoundType = GameRoundType.Posting;
			await _bettingLogic.PlaceBlinds();


			//Dispencing 1 card row
			GameRoundType = GameRoundType.Dispensing1;
			//CHECK: send dispence event
			await DispenceOneCircle(dispence1);


			//dispencing pause
			await Task.Delay(new Random().Next(100, 250));


			//Dispencing 2 card row
			GameRoundType = GameRoundType.Dispensing2;
			//CHECK: send second dispence event
			await DispenceOneCircle(dispence2);


			// Pre-flop -> blinds -> betting
			GameRoundType = GameRoundType.PreFlop;
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

#if DEBUG
			//CHECK: send event to open cards
			LobbySendHandle.ShowAllCards(LobbyClient.Instance.Id, dispence1, dispence2, playerIds);
#endif

			await Task.Delay(2000);

			foreach (var player in _players)
			{
				player.EndHand(new EndHandContext(_showdownCards));
			}
		}

		private async Task DispenceOneCircle(List<Card> cards)
		{
			Random random = new Random();

			for (int i = 0, j = 0; i < _players.Count; i++, j += 2)
			{
#if DEBUG
				LobbySendHandle.GiveCard(LobbyClient.Instance.Id, _players[i].ServerId, (int)cards[i].Type, (int)cards[i].Suit);
#endif

				await Task.Delay(random.Next(30, 250));
			}
		}

		private void DetermineWinnerAndAddPot(int pot, Pot mainPot, List<Pot> sidePot)
		{
			if (_players.Count(x => x.PlayerMoney.InHand) == 1)
			{
				var winner = _players.FirstOrDefault(x => x.PlayerMoney.InHand);
				GiveWin(winner, pot);
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
						GiveWin(_players[0], pot);
					}
					else if (betterHand < 0)
					{
						GiveWin(_players[1], pot);
					}
					else
					{
						GiveWin(_players[0], pot / 2);
						GiveWin(_players[1], pot / 2);
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
										GiveWin(_players.First(x => x.Name == nominee), prize);
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

		private void GiveWin(RealPlayerDecorator player, int amount)
		{
#if DEBUG
			//CHECK: send win amount event
			LobbySendHandle.WinAmount(LobbyClient.Instance.Id, player.ServerId, amount);
#endif
			player.PlayerMoney.Money += amount;
#if DEBUG
			LobbySendHandle.ShowPlayerMoney(LobbyClient.Instance.Id, player.ServerId, player.PlayerMoney.Money);
#endif
		}

		private async Task PlayRound(GameRoundType gameRoundType, int[] communityCardsIndexes = null)
		{
			if (communityCardsIndexes != null)
			{
				//CHECK: sending table cards info
				List<int> types = new List<int>();
				List<int> suits = new List<int>();
				for (var i = 0; i < communityCardsIndexes.Length; i++)
				{
					Card card = _deck.GetNextCard();
					types.Add((int)card.Type);
					suits.Add((int)card.Suit);
					_communityCards.Add(card);
				}

#if DEBUG
				LobbySendHandle.ShowTableCards(LobbyClient.Instance.Id, types.ToArray(), suits.ToArray(), communityCardsIndexes);
#endif
			}

			foreach (var player in _players)
			{
#if DEBUG
				//TODO: Player State At Start Of Round
				//LobbySendHandle.stat(LobbyClient.Instance.Id, player.ServerId, player.PlayerMoney.Money, _bettingLogic.Pot);
#endif

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
#if DEBUG
				//CHECK: update bank
				LobbySendHandle.ShowBank(LobbyClient.Instance.Id, _bettingLogic.Pot);
#endif
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
