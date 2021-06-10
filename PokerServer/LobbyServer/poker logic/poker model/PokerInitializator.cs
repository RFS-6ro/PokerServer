using GameCore.Poker.Model;
using LobbyServer.Client;
using LobbyServer.Client.Handlers;
using LobbyServer.pokerlogic.controllers;
using LobbyServer.pokerlogic.pokermodel.Players;
using LobbyServer.pokerlogic.pokermodel.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TexasHoldem.Logic.Extensions;
using UniCastCommonData;
using UniCastCommonData.Network.MessageHandlers;
using UniCastCommonData.Packet.InitialDatas;

public class PokerInitializator : IStaticInstance<PokerInitializator>
{
	public event Action OnGameEnds;

	public const int MaxPlayers = 9;

	public List<ServerPlayer> CurrentPlayers = new(MaxPlayers);
	public List<ConsoleUiDecorator> Decorators = new(MaxPlayers);

	public TableViewModel TableViewModel;

	public List<ChairViewModel> Chairs;

	private List<ServerPlayer> _readyToPlay = new(MaxPlayers);

	private Lobby_Client_Server Server => IStaticInstance<Lobby_Client_Server>.Instance;
	private SessionSender<Lobby_Client_Server> Sender => IStaticInstance<Lobby_Client_Server>.Instance.SendHandler;

	private TexasHoldemGame _currentGame;

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
		Sender.Multicast(CurrentPlayers.Where((x) => x != null).Select((x) => x.Guid),
						 new NewPlayerConnectSendingData(
							 300,//TODO: RECEIVE MONEY FROM DATABASE
							 name,
							 guid,
							 Guid.Empty,
							 Server.Id,
							 Server.ServerType,
							 (int)lobbyTOclient.NewPlayerConnect),
						 null);
		//SEND: join event

		_readyToPlay.Add(new ServerPlayer(guid, name));

		if (_currentGame != null)
		{
			Sender.SendAsync(new CurrentGameStateSendingData(
							 _currentGame.CollectData(),
							 guid,
							 Server.Id,
							 Server.ServerType,
							 (int)lobbyTOclient.CurrentGameState),
							 null);
		}

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

		Sender.Multicast(CurrentPlayers.Where((x) => x != null).Select((x) => x.Guid),
						 new PlayerDisconnectSendingData(
							 guid,
							 Guid.Empty,
							 Server.Id,
							 Server.ServerType,
							 (int)lobbyTOclient.PlayerDisconnect),
						 null);
		//SEND: disconnect event
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

		_currentGame = new TexasHoldemGame(players, TableViewModel);
		var winner = await _currentGame.Start();//wait for end of game

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
