using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Network;
using PokerSynchronisation;

namespace PokerLobby
{
	class Program
	{
		public static int MinimumPlayersNumberToStart { get; private set; } = 5;

		public static int ConnectedPlayers => LobbyClient.Players.Count((x) => x.IsReady);

		private static bool _isRunning = false;

		public static async Task Main(string[] args)
		{
			Thread mainThread = new Thread(new ThreadStart(MainThread));
			mainThread.Start();

#if DEBUG
			LobbyClient.Players.Add(new RealPlayer("1", 1));
			LobbyClient.Players.Add(new RealPlayer("2", 2));
			LobbyClient.Players.Add(new RealPlayer("3", 3));
			LobbyClient.Players.Add(new RealPlayer("4", 4));
			LobbyClient.Players.Add(new RealPlayer("5", 5));
			LobbyClient.Players.Add(new RealPlayer("6", 6));
#else
			string lobbyName;

			LobbyIdentifierData data = new LobbyIdentifierData();

			try
			{
				data.FromString(args?[0]);

				lobbyName = data.Name;
				if (data.NumberOfPlayers != 0)
				{
					MinimumPlayersNumberToStart = data.NumberOfPlayers;
				}
			}
			catch
			{
				lobbyName = DefaultSyncValues.LobbyName;
			}

			ConsoleLogger.Instance.Print($"Starting Lobby with name { lobbyName }");

			LobbyClient.Instance.SetName(lobbyName);

			LobbyClient.Instance.ConnectToServer();
#endif
			//await Task.Run(() =>
			//{
			//	while (true)
			//	{
			//		Task.Delay(100);
			//		if (LobbyClient.Instance.IsConnected == true)
			//		{
			//			return;
			//		}
			//	}
			//});

			await PlayersConnection();

			//System.Diagnostics.Process.Start("open", "-n -a Terminal");
			if (ConnectedPlayers >= MinimumPlayersNumberToStart)
			{
#if !DEBUG
				ConsoleLogger.Instance.Print("Starting Game Loop");
#endif
				// Starting Game with current players
				//CHECK: Initialize Game logic with small bling, buyIn etc
				await LobbyClient.Instance.PerformGameLoop(
#if !DEBUG
					data.BuyIn, data.SmallBlind
#else
					300, 3
#endif
					);
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
					while (ConnectedPlayers < MinimumPlayersNumberToStart ||
						   ConnectedPlayers != LobbyClient.Players.Count)
					{
						if (previousNumber != ConnectedPlayers)
						{
							previousNumber = ConnectedPlayers;
							waitingTime = 0;
						}

						await Task.Delay(500);
						waitingTime += 500;

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

				//CHECK: Show 10 sec Timer to players and await for it's ending
				LobbySendHandle.TimerEvent(LobbyClient.Instance.Id, DefaultSyncValues.LobbyTimerId, true, DefaultSyncValues.StartDelay);
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
					}
				}
				catch (Exception ex)
				{
					ConsoleLogger.Instance.PrintError(ex.ToString());
					Console.ReadKey();
				}
			}
		}

		private static void MainThread()
		{
			Console.WriteLine("Main thread started. Running at 30 ticks per second.");
			DateTime _nextLoop = DateTime.Now;

			while (_isRunning)
			{
				while (_nextLoop < DateTime.Now)
				{
					// If the time for the next loop is in the past, aka it's time to execute another tick
					IThreadManager.UpdateMain(); // Execute game logic

					_nextLoop = _nextLoop.AddMilliseconds(1000f / 30); // Calculate at what point in time the next tick should be executed

					if (_nextLoop > DateTime.Now)
					{
						// If the execution time for the next tick is in the future, aka the server is NOT running behind
						Thread.Sleep(_nextLoop - DateTime.Now); // Let the thread sleep until it's needed again.
					}
				}
			}
		}
	}
}
