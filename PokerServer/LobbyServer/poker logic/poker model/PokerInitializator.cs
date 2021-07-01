using GameCore.Poker.Model;
using LobbyServer.Client;
using LobbyServer.Client.Handlers;
using LobbyServer.pokerlogic.controllers;
using LobbyServer.pokerlogic.Extensions;
using LobbyServer.pokerlogic.pokermodel.Players;
using LobbyServer.pokerlogic.pokermodel.UI;
using ServerDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniCastCommonData;
using UniCastCommonData.Network.MessageHandlers;
using UniCastCommonData.Packet.InitialDatas;

/*

	StaticLogger.Print($"Poker Initializator + {Server.Id.ToString().Split('-')[0]}", "");
	StaticLogger.Print($"Poker Initializator + {Server.Id.ToString().Split('-')[0]}",
		new string[]
		{
			"multicasting for all users"
		}
	);
22
 */
public class PokerInitializator : StaticInstance<PokerInitializator>
{
	public event Action OnGameEnds;

	public const int MaxPlayers = 9;

	public List<ServerPlayer> CurrentPlayers = new(MaxPlayers);
	public List<ConsoleUiDecorator> Decorators = new(MaxPlayers);

	public TableViewModel TableViewModel;

	public List<ChairViewModel> Chairs;

	private Lobby_Client_Server Server => Lobby_Client_Server.Instance;
	private SessionSender<Lobby_Client_Server> Sender => Lobby_Client_Server.Instance.SendHandler;

	private TexasHoldemGame _currentGame;

	public PokerInitializator()
	{
		StaticLogger.Print($"Poker Initializator + {Server.Id.ToString().Split('-')[0]}", "poker initialisation");
		TableViewModel = new TableViewModel(1, 30);
		Instance = this;
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
		int playerMoney = 300; //TODO: RECEIVE MONEY FROM DATABASE
		var player = new ServerPlayer(guid, name, playerMoney);
		int randomSeatIndex = RandomProvider.Next(0, MaxPlayers);

		while (CurrentPlayers[randomSeatIndex] != null)
		{
			randomSeatIndex = RandomProvider.Next(0, MaxPlayers);
		}

		StaticLogger.Print($"Poker Initializator + {Server.Id.ToString().Split('-')[0]}", $"new player connectionw ith name {name} and id = {guid}, Money = {playerMoney}, Index = {randomSeatIndex}");
		CurrentPlayers[randomSeatIndex] = player;
		Decorators[randomSeatIndex].SetPlayer(player);
		Decorators[randomSeatIndex].DrawGameBox();

		Sender.MulticastExept(CurrentPlayers.Where((x) => x != null).Select((x) => x.Guid),
						 new NewPlayerConnectSendingData(
							 playerMoney,
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
		List<string> currentPlayersLog = new List<string>();
		for (int i = 0; i < CurrentPlayers.Count; ++i)
		{
			ServerPlayer activePlayer = CurrentPlayers[i];

			PlayerData activePlayerData = null;
			Guid activePlayerGuid;
			string log = $"{i}) ";

			if (activePlayer != null)// && )
			{
				if (_currentGame != null)
				{
					activePlayerData = _currentGame.CollectDataByGuid(activePlayer.Guid);
				}

				log += "active player ";

				if (activePlayerData == null)
				{
					log += "receiver ";
					activePlayerData = new PlayerData(activePlayer.Name, 300, 0, string.Empty, -1, i, false, false);
				}
				activePlayerData.Index = i;
				activePlayerGuid = activePlayer.Guid;
				log += $"[Name = {activePlayer.Name}, Id = {activePlayer.Guid}]";
			}
			else
			{
				activePlayerData = new PlayerData("Empty", -1, -1, string.Empty, -1, i, false, false);
				activePlayerGuid = Guid.Empty;
				log += "empty player chair";
			}

			currentPlayersLog.Add(log);
			datas.Add(new UniCastCommonData.Packet.InitialDatas.Tuple<Guid, PlayerData>(activePlayerGuid, activePlayerData));
		}

		StaticLogger.Print($"Poker Initializator + {Server.Id.ToString().Split('-')[0]}",
			new string[]
			{
				$"sending current game state to player {guid}"
			}
			.Concat(currentPlayersLog)
		);

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

			StaticLogger.Print($"Poker Initializator + {Server.Id.ToString().Split('-')[0]}", $"sending diconnecting of player {guid} event");

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
		else
		{
			StaticLogger.Print($"Poker Initializator + {Server.Id.ToString().Split('-')[0]}", $"diconnecting player {guid} not found on server");
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
		if (CurrentPlayers.Count((x) => x != null) < 2)
		{
			StaticLogger.Print($"Poker Initializator + {Server.Id.ToString().Split('-')[0]}", $"awaiting for at least 2 players");
		}

		while (CurrentPlayers.Count((x) => x != null) < 2)
		{
			await Task.Delay(100);
		}

		var players = PreparePlayers();

		_currentGame = new TexasHoldemGame(players, TableViewModel);
		StaticLogger.Print($"Poker Initializator + {Server.Id.ToString().Split('-')[0]}",
			new string[]
			{
				"starting new game"
			}
			.Concat(GetPlayersForLogs(players))
		);

		var winner = await _currentGame.Start();//wait for end of game

		if (CurrentPlayers.Count((x) => x != null) >= 2)
		{
			StaticLogger.Print($"Poker Initializator + {Server.Id.ToString().Split('-')[0]}",
			new string[]
			{
				$"initing new round"
			}
			.Concat(GetPlayersForLogs())
		);
			await Init();
			return;
		}

		foreach (var player in CurrentPlayers)
		{
			if (player != null)
			{
				//TODO: uncomment StaticInstance<Lobby_Client_Server>.Instance.FindSession(player.Guid)?.Disconnect();
			}
		}
	}

	private List<ConsoleUiDecorator> PreparePlayers()
	{
		StaticLogger.Print($"Poker Initializator + {Server.Id.ToString().Split('-')[0]}", $"removing empty decorators and shifting");
		CurrentPlayers.Add(CurrentPlayers[0]);
		CurrentPlayers.RemoveAt(0);
		Decorators.Add(Decorators[0]);
		Decorators.RemoveAt(0);
		return Decorators.Where((x) => x.PlayerGuid != Guid.Empty).ToList();
	}

	internal IEnumerable<string> GetPlayersForLogs(IEnumerable<ConsoleUiDecorator> players = null, Func<ConsoleUiDecorator, string> getAdditionalInfo = null)
	{
		if (players == null)
		{
			players = Decorators;
		}
		List<string> logPlayers = new List<string>();

		int index = 0;
		foreach (var player in players)
		{
			string additionalInfo = getAdditionalInfo?.Invoke(player);
			logPlayers.Add($"{index}) {player.Name} {player.PlayerGuid}" + additionalInfo ?? string.Empty);
			index++;
		}

		return logPlayers;
	}
}
