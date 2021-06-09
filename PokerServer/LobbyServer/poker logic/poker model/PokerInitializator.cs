using GameCore.Poker.Model;
using GameCore.Poker.Model.Player;
using LobbyServer.Client;
using LobbyServer.pokerlogic.controllers;
using LobbyServer.pokerlogic.pokermodel.Players;
using LobbyServer.pokerlogic.pokermodel.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TexasHoldem.Logic.Extensions;
using TexasHoldem.Logic.Players;
using UniCastCommonData;

public class PokerInitializator : IStaticInstance<PokerInitializator>
{
	public event Action OnGameEnds;

	public const int MaxPlayers = 9;

	public List<ServerPlayer> CurrentPlayers = new(MaxPlayers);
	public List<ConsoleUiDecorator> Decorators = new(MaxPlayers);

	public TableViewModel TableViewModel;

	public List<ChairViewModel> Chairs;

	private List<ServerPlayer> _readyToPlay = new(MaxPlayers);

	public PokerInitializator()
	{
		IStaticInstance<PokerInitializator>.Instance = this;
		//init seats
		for (int i = 0; i < MaxPlayers; i++)
		{
			CurrentPlayers.Add(null);
			ConsoleUiDecorator decorator = new ConsoleUiDecorator();
			decorator.DrawGameBox((6 * i) + 3, 30, 1);
			Decorators.Add(decorator);
		}
	}

	public void AddPlayer(Guid guid, string name)
	{
		_readyToPlay.Add(new ServerPlayer(guid, name));
		//TODOSEND: join event
	}

	public void RemovePlayer(Guid guid)
	{
		var disconnectingPlayer = CurrentPlayers.Find((x) => x.Guid == guid);
		if (disconnectingPlayer != null)
		{
			int index = CurrentPlayers.IndexOf(disconnectingPlayer);
			CurrentPlayers[index] = null;
			Decorators[index].SetPlayer(null);
		}
		else
		{
			disconnectingPlayer = _readyToPlay.Find((x) => x.Guid == guid);
			if (disconnectingPlayer != null)
			{
				_readyToPlay.Remove(disconnectingPlayer);
			}
		}
		//TODOSEND: disconnect event
	}

	public async Task Init()
	{
		for (int i = 0; i < _readyToPlay.Count; i++)
		{
			int randomSeatIndex = RandomProvider.Next(0, MaxPlayers);

			while (CurrentPlayers[randomSeatIndex] != null)
			{
				randomSeatIndex = RandomProvider.Next(0, MaxPlayers);
			}

			CurrentPlayers[randomSeatIndex] = _readyToPlay[i];
			Decorators[randomSeatIndex].SetPlayer(_readyToPlay[i]);
		}

		_readyToPlay.Clear();

		var players = PreparePlayers();

		var winner = await new TexasHoldemGame<ConsoleUiDecorator>(players, TableViewModel).Start();//wait for end of game

		if (_readyToPlay.Count + CurrentPlayers.Count((x) => x != null) > 2)
		{
			await Init();
			return;
		}

		foreach (var player in CurrentPlayers)
		{
			if (player != null)
			{
				IStaticInstance<Lobby_Client_Server>.Instance.FindSession(player.Guid)?.Disconnect();
			}
		}

		return;






		//Random random = new Random();

		//int playerCount = 9;
		//int randomPlayerCount = random.Next(2, playerCount);
		//int shiftNumber = random.Next(0, randomPlayerCount - 1);

		//List<IPlayer> players = new List<IPlayer>();
		//List<UnityPlayerDecorator> decorators = new List<UnityPlayerDecorator>();

		//for (int i = 0; i < playerCount; i++)
		//{
		//	IPlayer player;
		//	player = new BotPlayer();
		//	UnityPlayerDecorator decorator = new ServerPlayerDecorator(Chairs[i]);

		//	decorator.SetPlayer(player);
		//	players.Add(player);
		//	decorators.Add(decorator);
		//}

		//for (int i = 0; i < playerCount - randomPlayerCount; i++)
		//{
		//	int randomPlayerIndex = random.Next(1, players.Count);
		//	players.RemoveAt(randomPlayerIndex);
		//	decorators.RemoveAt(randomPlayerIndex);
		//}

		//for (int i = 0; i <= shiftNumber; i++)
		//{
		//	players.Add(players[0]);
		//	players.RemoveAt(0);
		//	decorators.Add(decorators[0]);
		//	decorators.RemoveAt(0);
		//}

		//for (int i = 0; i < decorators.Count; i++)
		//{
		//	decorators[i].ConfigureSeat();
		//}











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


	}

	private List<ConsoleUiDecorator> PreparePlayers()
	{
		CurrentPlayers.Add(CurrentPlayers[0]);
		CurrentPlayers.RemoveAt(0);
		Decorators.Add(Decorators[0]);
		Decorators.RemoveAt(0);
		return Decorators.Where((x) => x != null).ToList();
	}
}
