using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Network;
using PokerSynchronisation;
using TexasHoldem.AI.DummyPlayer;
using TexasHoldem.Logic.GameMechanics;
using TexasHoldem.Logic.Players;
using TexasHoldem.UI.Console;

namespace PokerLobby
{
	class Program
	{
		public static EventWaitHandle EventHandle;

		private static ITexasHoldemGame _game;
		private static List<IPlayer> _players;

		private const int GameWidth = 66;
		private const int NumberOfCommonRows = 3; // place for community cards, pot, main pot, side pots

		private static readonly List<IPlayer> Players = new List<IPlayer>();

		public static int MinimumPlayersNumberToStart { get; private set; } = 5;

		public static int ConnectedPlayers => Players.Count;//((x) => x.IsReady);

		public static async Task Main(string[] args)
		{
			Players.Add(new DummyPlayer());
			Players.Add(new DummyPlayer());
			Players.Add(new DummyPlayer());
			Players.Add(new ConsolePlayer((6 * Players.Count) + NumberOfCommonRows));
			Players.Add(new DummyPlayer());
			Players.Add(new DummyPlayer());
			Players.Add(new DummyPlayer());

			{
				//string eventHandlerName;

				//try
				//{
				//	eventHandlerName = args[0];
				//}
				//catch
				//{
				//	eventHandlerName = DefaultSyncValues.EventHandlerName;
				//}

				//EventHandle = EventWaitHandle.OpenExisting(eventHandlerName);
			}

			await PlayersConnection();

			//System.Diagnostics.Process.Start("open", "-n -a Terminal");
			if (ConnectedPlayers >= MinimumPlayersNumberToStart)
			{
				_players = AssignRealPlayersToInternalDecorators();

				ConsoleLogger.Instance.Print("starting server");
				//TODO: Start Game with current players
				_game = new TexasHoldemGame<RealPlayerDecorator>(_players);
				_game.Start();

			}
		}

		public async static Task PlayersConnection()
		{
			int waitingTime = 0;
			int previousNumber = ConnectedPlayers;
			bool isReadyToStart = false;

			while (isReadyToStart == false)
			{
				await Task.Run(() =>
				{
					try
					{
						while (ConnectedPlayers < MinimumPlayersNumberToStart || ConnectedPlayers != Players.Count)
						{
							if (previousNumber != ConnectedPlayers)
							{
								previousNumber = ConnectedPlayers;
								waitingTime = 0;
							}

							Thread.Sleep(500);
							waitingTime += 500;
							ConsoleLogger.Instance.Print(waitingTime.ToString());

							if (waitingTime > DefaultSyncValues.LobbyWaitingLimit)
							{
								//TODO: terminate lobby and free it
								return;
							}
						}
					}
					catch (System.Exception ex)
					{
						ConsoleLogger.Instance.PrintError(ex.ToString());
						Console.ReadKey();
					}
				});

				//TODO: Show 10 sec Timer to players and await for it's ending
				ConsoleLogger.Instance.Print("waiting 10 sec");
				await Task.Run(() =>
				{
					try
					{
						for (int i = 0; i < DefaultSyncValues.StartDelay; i += 1000)
						{
							if (ConnectedPlayers != previousNumber)
							{
								previousNumber = ConnectedPlayers;
								waitingTime = 0;

								isReadyToStart = false;
								break;
							}
							else
							{
								isReadyToStart = true;
							}

							Thread.Sleep(100);
							ConsoleLogger.Instance.Print(i.ToString());
						}
					}
					catch (System.Exception ex)
					{
						ConsoleLogger.Instance.PrintError(ex.ToString());
						Console.ReadKey();
					}

				});
			}
		}

		private static List<IPlayer> AssignRealPlayersToInternalDecorators()
		{
			List<IPlayer> players = new List<IPlayer>();
			//TODO: Add players to Holdem

			for (int i = 0; i < Players.Count; i++)
			{
				ConsoleUiDecorator decorator = new ConsoleUiDecorator();
				decorator.SetPlayer(Players[i]);
				decorator.DrawGameBox((6 * i) + NumberOfCommonRows, GameWidth, 1);
				players.Add(decorator);
			}

			return players;
		}
	}
}
