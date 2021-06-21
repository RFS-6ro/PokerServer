using LobbyServer.Client;
using LobbyServer.Client.Handlers;
using LobbyServer.pokerlogic.controllers;
using LobbyServer.pokerlogic.pokermodel.Players;
using LobbyServer.pokerlogic.pokermodel.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LobbyServer.pokerlogic.GameMechanics;
using UniCastCommonData;
using UniCastCommonData.Network.MessageHandlers;
using UniCastCommonData.Packet.InitialDatas;

namespace GameCore.Poker.Model
{
	public class TexasHoldemGame
	{
		protected static readonly int[] SmallBlinds =
		{
			1, 2, 3, 5, 10, 15, 20, 25, 30, 40, 50, 60, 80, 100, 150, 200, 300,
			400, 500, 600, 800, 1000, 1500, 2000, 3000, 4000, 5000, 6000, 8000,
			10000, 15000, 20000, 30000, 40000, 50000, 60000, 80000, 100000,
		};

		public readonly ICollection<ConsoleUiDecorator> allPlayers;

		private readonly TableViewModel _tableViewModel;

		private int initialMoney;

		public int HandsPlayed { get; set; }

		private Lobby_Client_Server Server => IStaticInstance<Lobby_Client_Server>.Instance;
		private SessionSender<Lobby_Client_Server> Sender => IStaticInstance<Lobby_Client_Server>.Instance.SendHandler;

		public TexasHoldemGame(IList<ConsoleUiDecorator> players, TableViewModel tableViewModel, int initialMoney = 200)
			: this(players, initialMoney)
		{
			// Ensure the players have unique names
			var duplicateNames = players.GroupBy(x => x)
				.Where(group => group.Count() > 1)
				.Select(group => group.Key.Name);

			if (tableViewModel == null)
			{
				throw new ArgumentNullException();
			}

			if (duplicateNames.Count() > 0)
			{
				throw new ArgumentException($"Players have the same name: \"{string.Join(" ", duplicateNames.ToArray())}\"");
			}

			_tableViewModel = tableViewModel;
		}

		private TexasHoldemGame(ICollection<ConsoleUiDecorator> players, int initialMoney = 1000)
		{
			if (players == null)
			{
				throw new ArgumentNullException(nameof(players));
			}

			if (players.Count < 2 || players.Count > 9)
			{
				throw new ArgumentOutOfRangeException(nameof(players), $"The number of players must be from 2 to {9}");
			}

			if (initialMoney <= 0 || initialMoney > 200000)
			{
				throw new ArgumentOutOfRangeException(nameof(initialMoney), "Initial money should be greater than 0 and less than 200000");
			}

			allPlayers = new List<ConsoleUiDecorator>(players.Count);
			foreach (var player in players)
			{
				//ConsoleUiDecorator player = new ConsoleUiDecorator(chairs[0]);
				//player.SetPlayer(item);
				allPlayers.Add(player);
			}

			this.initialMoney = initialMoney;
			HandsPlayed = 0;
		}

		public PlayerData CollectDataByGuid(Guid guid)
		{
			ConsoleUiDecorator player = allPlayers.FirstOrDefault((x) => x.PlayerGuid == guid);
			if (player != null)
			{
				PlayerData playerData = new PlayerData(
					player.Name,
					player.PlayerMoney.Money,
					player.PlayerMoney.CurrentRoundBet,
					player.PlayerMoney.LastPlayerAction.ToString(),
					player.PlayerMoney.LastPlayerAction.Money,
					-1,
					player.Cards.Count != 0,
					player.IsDealer
				);

				return playerData;
			}

			return null;
		}

		public async Task<ConsoleUiDecorator> Start()
		{
			var playerNames = allPlayers.Select(x => x.Name).ToList().AsReadOnly();
			foreach (var player in allPlayers)
			{
				player.StartGame(new StartGameContext(playerNames, player.BuyIn == -1 ? initialMoney : player.BuyIn));

				Sender.SendAsync(new StartGameSendingData(
									 player.BuyIn == -1 ? initialMoney : player.BuyIn,
									 player.PlayerGuid,
									 Server.Id,
									 Server.ServerType,
									 (int)lobbyTOclient.StartGame),
								 null);
			}

			await PlayGame();

			var winner = allPlayers.WithMoney().FirstOrDefault();

			foreach (var player in allPlayers)
			{
				player.EndGame(new EndGameContext(winner.Name));
				Sender.SendAsync(new EndGameSendingData(
									 winner.PlayerGuid,
									 player.PlayerGuid,
									 Server.Id,
									 Server.ServerType,
									 (int)lobbyTOclient.EndGame),
								 null);
			}

			return winner;
		}

		private void Rebuy()
		{
			var playerNames = allPlayers.Select(x => x.Name).ToList().AsReadOnly();
			foreach (var player in allPlayers)
			{
				if (player.PlayerMoney.Money <= 0)
				{
					player.StartGame(new StartGameContext(playerNames, player.BuyIn == -1 ? initialMoney : player.BuyIn));
					Sender.SendAsync(new StartGameSendingData(
										 player.BuyIn == -1 ? initialMoney : player.BuyIn,
										 player.PlayerGuid,
										 Server.Id,
										 Server.ServerType,
										 (int)lobbyTOclient.StartGame),
									 null);
				}
			}
		}

		private async Task PlayGame()
		{
			var shifted = allPlayers.ToList();

			int shuffleNumber = new Random().Next(0, allPlayers.Count - 1);

			for (int i = 0; i < shuffleNumber; i++)
			{
				shifted = shifted.WithMoney().Cast<ConsoleUiDecorator>().ToList();
				shifted.Add(shifted.First());
				shifted.RemoveAt(0);
			}

			// While at least two players have money
			if (allPlayers.WithMoney().Count() > 1)
			{
				HandsPlayed++;

				//Every 10 hands the blind increases
				var smallBlind = SmallBlinds[7];
				//var smallBlind = SmallBlinds[0];

				// Players are shifted in order of priority to make a move
				shifted = shifted.WithMoney().Cast<ConsoleUiDecorator>().ToList();
				shifted.Add(shifted.First());
				shifted.RemoveAt(0);

				// Rotate players
				HandLogic hand = new HandLogic(shifted, HandsPlayed, smallBlind, _tableViewModel);

				await hand.Play();

				//Rebuy();
			}
		}
	}
}
