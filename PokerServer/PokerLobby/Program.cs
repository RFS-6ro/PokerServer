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
		private static TexasHoldemGameLogic _game;
		private static List<IPlayer> _players;

#if DEBUG
		private const int GameWidth = 66;
		private const int NumberOfCommonRows = 3; // place for community cards, pot, main pot, side pots
#endif

		public static List<IPlayer> Players = new List<IPlayer>();

		public static int MinimumPlayersNumberToStart { get; private set; } = 5;

		public static int ConnectedPlayers => Players.Count;//((x) => x.IsReady);

		public static async Task Main(string[] args)
		{

#if DEBUG
			Players.Add(new DummyPlayer());
			Players.Add(new DummyPlayer());
			Players.Add(new DummyPlayer());
			Players.Add(new ConsolePlayer((6 * Players.Count) + NumberOfCommonRows));
			Players.Add(new DummyPlayer());
			Players.Add(new DummyPlayer());
			Players.Add(new DummyPlayer());
#else
			string lobbyName;

			try
			{
				lobbyName = args[0];
			}
			catch
			{
				lobbyName = DefaultSyncValues.LobbyName;
			}

			ConsoleLogger.Instance.PrintSuccess($"Starting Lobby with name { lobbyName }");

			LobbyClient.Instance.SetName(lobbyName);
			LobbyClient.Instance.ConnectToServer();
#endif

			await PlayersConnection();

			//System.Diagnostics.Process.Start("open", "-n -a Terminal");
			if (ConnectedPlayers >= MinimumPlayersNumberToStart)
			{
				_players = AssignRealPlayersToInternalDecorators();

#if !DEBUG
				ConsoleLogger.Instance.Print("Starting Game Loop");
#endif
				// Starting Game with current players
				_game = new TexasHoldemGameLogic(_players);
				await _game.Start();
				//TODO: Disconnect winner player from lobby 
			}
		}

		public async static Task PlayersConnection()
		{
			int waitingTime = 0;
			int previousNumber = ConnectedPlayers;
			bool isReadyToStart = false;

			while (isReadyToStart == false)
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

						await Task.Delay(500);
						waitingTime += 500;
#if !DEBUG
						ConsoleLogger.Instance.Print(waitingTime.ToString());
#endif

						if (waitingTime > DefaultSyncValues.LobbyWaitingLimit)
						{
							//TODO: terminate lobby and free it
							return;
						}
					}
				}
				catch (Exception ex)
				{
					ConsoleLogger.Instance.PrintError(ex.ToString());
					Console.ReadKey();
				}

				//TODO: Show 10 sec Timer to players and await for it's ending
#if !DEBUG
				ConsoleLogger.Instance.Print("waiting 10 sec");
#endif

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

						await Task.Delay(100);
#if !DEBUG
						ConsoleLogger.Instance.Print(i.ToString());
#endif
					}
				}
				catch (Exception ex)
				{
					ConsoleLogger.Instance.PrintError(ex.ToString());
					Console.ReadKey();
				}
			}
		}

		private static List<IPlayer> AssignRealPlayersToInternalDecorators()
		{
			List<IPlayer> players = new List<IPlayer>();
			//TODO: Add players to Holdem

			for (int i = 0; i < Players.Count; i++)
			{
				RealPlayerDecorator decorator = new RealPlayerDecorator();
#if DEBUG
				decorator.DrawGameBox((6 * i) + NumberOfCommonRows, GameWidth, 1);
				//				ConsoleUiDecorator decorator = new ConsoleUiDecorator();
				//#else
#endif
				decorator.SetPlayer(Players[i]);
				players.Add(decorator);
			}

			return players;
		}
	}
}
