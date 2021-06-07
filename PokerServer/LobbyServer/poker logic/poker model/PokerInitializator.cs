using GameCore.Poker.Model;
using GameCore.Poker.Model.Player;
using LobbyServer.pokerlogic.controllers;
using LobbyServer.pokerlogic.pokermodel.Players;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TexasHoldem.Logic.Extensions;
using TexasHoldem.Logic.Players;
using TexasHoldem.UI.Console;

public class PokerInitializator
{
	public event Action OnGameEnds;

	public const int MaxPlayers = 9;

	public List<ServerPlayer> CurrentPlayers = new(MaxPlayers);
	public List<ConsoleUiDecorator> Decorators = new(MaxPlayers);

	public PokerInitializator(List<ServerPlayer> initialPlayers)
	{
		//init seats
		for (int i = 0; i < MaxPlayers; i++)
		{
			CurrentPlayers.Add(null);
			ConsoleUiDecorator decorator = new ConsoleUiDecorator();
			decorator.DrawGameBox((6 * i) + 3, 30, 1);
			Decorators.Add(decorator);
		}
		return;

		for (int i = 0; i < initialPlayers.Count; i++)
		{
			int randomSeatIndex = RandomProvider.Next(0, MaxPlayers);
			while (CurrentPlayers[randomSeatIndex] != null)
			{
				randomSeatIndex = RandomProvider.Next(0, MaxPlayers);
			}
			CurrentPlayers[randomSeatIndex] = initialPlayers[i];
		}


	}











	public TableViewModel TableViewModel;

	public List<ChairViewModel> Chairs;

	public async Task Init(object[] initParams = null)
	{
		Random random = new Random();

		int playerCount = 9;
		int randomPlayerCount = random.Next(2, playerCount);
		int shiftNumber = random.Next(0, randomPlayerCount - 1);

		List<IPlayer> players = new List<IPlayer>();
		List<UnityPlayerDecorator> decorators = new List<UnityPlayerDecorator>();

		for (int i = 0; i < playerCount; i++)
		{
			IPlayer player;
			player = new BotPlayer();
			UnityPlayerDecorator decorator = new ServerPlayerDecorator(Chairs[i]);

			decorator.SetPlayer(player);
			players.Add(player);
			decorators.Add(decorator);
		}

		for (int i = 0; i < playerCount - randomPlayerCount; i++)
		{
			int randomPlayerIndex = random.Next(1, players.Count);
			players.RemoveAt(randomPlayerIndex);
			decorators.RemoveAt(randomPlayerIndex);
		}

		for (int i = 0; i <= shiftNumber; i++)
		{
			players.Add(players[0]);
			players.RemoveAt(0);
			decorators.Add(decorators[0]);
			decorators.RemoveAt(0);
		}

		for (int i = 0; i < decorators.Count; i++)
		{
			decorators[i].ConfigureSeat();
		}
		//chairs.RemoveRange(shiftNumber, playerCount - randomPlayerCount);

		//int index = 0;

		//for (int i = playerCount - shiftNumber + 1; i >= 1; i--)
		//{
		//	UnityPlayerDecorator predDecorator = new BotPlayerDecorator(chairs[i], BotNames[i]);

		//	predDecorator.SetPlayer(players[index]);
		//	decorators.Add(predDecorator);
		//	index++;
		//}

		//UnityPlayerDecorator mainOlayerDecorator = new RealPlayerDecorator(chairs[0], null);

		//mainOlayerDecorator.SetPlayer(players[index]);
		//decorators.Add(mainOlayerDecorator);

		//for (int i = 1; i < randomPlayerCount - shiftNumber; i++)
		//{
		//	UnityPlayerDecorator predDecorator = new BotPlayerDecorator(chairs[i], BotNames[i]);

		//	predDecorator.SetPlayer(players[index]);
		//	decorators.Add(predDecorator);
		//	index++;
		//}


		var game = new TexasHoldemGame<UnityPlayerDecorator>(decorators, TableViewModel);
		var winner = await StartGame(game);
	}

	private async Task<TDECORATOR> StartGame<TDECORATOR>(TexasHoldemGame<TDECORATOR> game) where TDECORATOR : UnityPlayerDecorator, new()
	{
		return await game.Start();
	}
}
