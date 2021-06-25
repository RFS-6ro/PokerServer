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
using LobbyServer.pokerlogic.Extensions;
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

	private Lobby_Client_Server Server => IStaticInstance<Lobby_Client_Server>.Instance;
	private SessionSender<Lobby_Client_Server> Sender => IStaticInstance<Lobby_Client_Server>.Instance.SendHandler;

	private TexasHoldemGame _currentGame;

	public PokerInitializator()
	{
		TableViewModel = new TableViewModel(1, 30);
		IStaticInstance<PokerInitializator>.Instance = this;
		//init seats
		for (int i = 0; i < MaxPlayers; i++)
		{
			CurrentPlayers.Add(null);
			ConsoleUiDecorator decorator = new ConsoleUiDecorator((6 * i) + 3, 30);
			decorator.DrawGameBox();
			Decorators.Add(decorator);
		}
	}

	public void AddPlayer(Guid guid, string name)
	{
		var player = new ServerPlayer(guid, name, 300);
		int randomSeatIndex = RandomProvider.Next(0, MaxPlayers);

		while (CurrentPlayers[randomSeatIndex] != null)
		{
			randomSeatIndex = RandomProvider.Next(0, MaxPlayers);
		}

		CurrentPlayers[randomSeatIndex] = player;
		Decorators[randomSeatIndex].SetPlayer(player);
		Decorators[randomSeatIndex].DrawGameBox();

		Sender.MulticastExept(CurrentPlayers.Where((x) => x != null).Select((x) => x.Guid),
						 new NewPlayerConnectSendingData(
							 300,//TODO: RECEIVE MONEY FROM DATABASE
							 name,
							 randomSeatIndex,
							 guid,
							 Guid.Empty,
							 Server.Id,
							 Server.ServerType,
							 (int)lobbyTOclient.NewPlayerConnect),
						 null,
						 guid);
		//SEND: join event

		//_readyToPlay.Add(new ServerPlayer(guid, name));

		List<UniCastCommonData.Packet.InitialDatas.Tuple<Guid, PlayerData>> datas = new List<UniCastCommonData.Packet.InitialDatas.Tuple<Guid, PlayerData>>();

		for (int i = 0; i < CurrentPlayers.Count; ++i)
		{
			ServerPlayer activePlayer = CurrentPlayers[i];

			PlayerData activePlayerData = null;
			Guid activePlayerGuid;

			if (activePlayer != null)// && )
			{
				if (_currentGame != null)
				{
					activePlayerData = _currentGame.CollectDataByGuid(activePlayer.Guid);
				}

				if (activePlayerData == null)
				{
					activePlayerData = new PlayerData(activePlayer.Name, 300, 0, string.Empty, -1, i, false, false);
				}
				activePlayerData.Index = i;
				activePlayerGuid = activePlayer.Guid;
			}
			else
			{
				activePlayerData = new PlayerData("Empty", -1, -1, string.Empty, -1, i, false, false);
				activePlayerGuid = Guid.Empty;
			}

			datas.Add(new UniCastCommonData.Packet.InitialDatas.Tuple<Guid, PlayerData>(activePlayerGuid, activePlayerData));
		}

		Sender.SendAsync(new CurrentGameStateSendingData(
							 datas,
							 guid,
							 Server.Id,
							 Server.ServerType,
							 (int)lobbyTOclient.CurrentGameState),
						 null);

	}

	public void RemovePlayer(Guid guid)
	{
		var disconnectingPlayer = CurrentPlayers.Find((x) => x != null && x.Guid == guid);
		if (disconnectingPlayer != null)
		{
			int index = CurrentPlayers.IndexOf(disconnectingPlayer);
			CurrentPlayers[index] = null;
			Decorators[index].SetPlayer(null);

			if (_currentGame != null)
			{
				_currentGame.allPlayers.FirstOrDefault((player) => player.PlayerGuid == guid)?.Disconnect();
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
	}

	public ConsoleUiDecorator FindPlayerDecoratorByGuid(Guid receiverGuid)
	{
		if (receiverGuid == Guid.Empty)
		{
			return null;
		}

		return Decorators.Find((x) => x.PlayerGuid == receiverGuid);
	}

	public async Task Init()
	{
		while (CurrentPlayers.Count((x) => x != null) < 2)
		{
			await Task.Delay(100);
		}

		var players = PreparePlayers();

		_currentGame = new TexasHoldemGame(players, TableViewModel);
		var winner = await _currentGame.Start();//wait for end of game

		if (CurrentPlayers.Count((x) => x != null) >= 2)
		{
			await Init();
			return;
		}

		foreach (var player in CurrentPlayers)
		{
			if (player != null)
			{
				//TODO: uncomment IStaticInstance<Lobby_Client_Server>.Instance.FindSession(player.Guid)?.Disconnect();
			}
		}
	}

	private List<ConsoleUiDecorator> PreparePlayers()
	{
		CurrentPlayers.Add(CurrentPlayers[0]);
		CurrentPlayers.RemoveAt(0);
		Decorators.Add(Decorators[0]);
		Decorators.RemoveAt(0);
		return Decorators.Where((x) => x.PlayerGuid != Guid.Empty).ToList();
	}
}
