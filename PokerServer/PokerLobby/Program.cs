using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Network;
using PokerSynchronisation;
using TexasHoldem.Logic.GameMechanics;
using TexasHoldem.Logic.Players;

namespace PokerLobby
{
	class Program
	{
		public static int MinimumPlayersNumberToStart { get; private set; } = 5;

		public static int ConnectedPlayers => LobbyClient.Players.Count((x) => x.IsReady);

		public static async Task Main(string[] args)
		{
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
			data.FromString(args[0]);

			try
			{
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

			await PlayersConnection();

			//System.Diagnostics.Process.Start("open", "-n -a Terminal");
			if (ConnectedPlayers >= MinimumPlayersNumberToStart)
			{
#if !DEBUG
				ConsoleLogger.Instance.Print("Starting Game Loop");
#endif
				//CHECK: Initialize Game logic with small bling, buyIn etc
				// Starting Game with current players
				await LobbyClient.Instance.PerformGameLoop(data.BuyIn, data.SmallBlind);
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
	}
}
