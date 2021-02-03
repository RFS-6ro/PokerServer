using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PokerSynchronisation;
using TexasHoldem.Logic.GameMechanics;
using TexasHoldem.Logic.Players;

namespace PokerLobby
{
	class Program
	{
		public static int ConnectedPlayers { get; set; } = 0;
		public static EventWaitHandle EventHandle;

		private static ITexasHoldemGame _game;
		private static List<IPlayer> _players;

		public static async Task Main(string[] args)
		{
			string eventHandlerName;
			if (args != null && args[0] != null)
			{
				eventHandlerName = args[0];
			}
			else
			{
				eventHandlerName = DefaultSyncValues.EventHandlerName;
			}

			EventHandle = EventWaitHandle.OpenExisting(args[0]);

			await Task.Run(() => PlayersConnection());
		}

		public async static void PlayersConnection(int minimumPlayersNumberToStart = 5)
		{
			int waitingTime = 0;
			int previousNumber = ConnectedPlayers;
			bool isReadyToStart = false;

			while (isReadyToStart == false)
			{
				await Task.Run(() =>
				{
					while (ConnectedPlayers < minimumPlayersNumberToStart)
					{
						if (previousNumber != ConnectedPlayers)
						{
							previousNumber = ConnectedPlayers;
							waitingTime = 0;
						}

						Thread.Sleep(500);
						waitingTime += 500;

						if (waitingTime < DefaultSyncValues.LobbyWaitingLimit)
						{
							//TODO: terminate lobby and free it
							return;
						}
					}
				});

				//TODO: Show 10 sec Timer to players and await for it's ending
				await Task.Run(() =>
				{
					for (int i = 0; i < DefaultSyncValues.StartDelay; i += 100)
					{
						if (ConnectedPlayers < minimumPlayersNumberToStart)
						{
							previousNumber = ConnectedPlayers;
							waitingTime = 0;

							isReadyToStart = false;
						}
						else
						{
							isReadyToStart = true;
						}

						Thread.Sleep(100);
					}

				});
			}

			if (ConnectedPlayers >= minimumPlayersNumberToStart)
			{
				_players = AssignRealPlayersToInternalDecorators();

				//TODO: Start Game with current players
				_game = new TexasHoldemGame(_players);


			}

		}

		private static List<IPlayer> AssignRealPlayersToInternalDecorators()
		{
			List<IPlayer> players = new List<IPlayer>();
			//TODO: Add players to Holdem

			for (int i = 0; i < ConnectedPlayers; i++)
			{
				players.Add(new RealPlayerDecorator());
			}

			return players;
		}
	}
}
