using System;
using System.Collections.Generic;
using System.Linq;
using TestingClient.Lobby;
using UniCastCommonData;
using UniCastCommonData.Packet.InitialDatas;

namespace TestingClient
{
	public class PokerInitializer : IStaticInstance<PokerInitializer>
	{
		public const int MaxPlayers = 9;

		public List<ConsoleUiDecorator> Decorators = new(MaxPlayers);

		public TableViewModel Table { get; }

		private ConsoleUiDecorator _mainPlayer;

		public PokerInitializer()
		{
			IStaticInstance<PokerInitializer>.Instance = this;
			Table = new TableViewModel(30);
			for (int i = 0; i < MaxPlayers; i++)
			{
				ConsoleUiDecorator decorator = new ConsoleUiDecorator();
				decorator.DrawGameBox((6 * i) + 3, 30);
				Decorators.Add(decorator);
			}
		}

		public void SetCurrentGameState(CurrentGameStateSendingData sendingData)
		{
			var keys = sendingData.Datas.Select((x) => x.Item1).ToList();

			int realPlayerIndex = keys.IndexOf(IStaticInstance<Client_Lobby>.Instance.Id);

			_mainPlayer = Decorators[realPlayerIndex];

			for (int i = realPlayerIndex; i < sendingData.Datas.Count; i++)
			{
				int index = i - realPlayerIndex;
				Decorators[index].SetPlayerData(sendingData.Datas[index].Item1, sendingData.Datas[index].Item2);
			}

			for (int i = 0; i < realPlayerIndex; i++)
			{
				int index = i + 5;
				Decorators[index].SetPlayerData(sendingData.Datas[index].Item1, sendingData.Datas[index].Item2);
			}
		}

		public ConsoleUiDecorator FindPlayerByGuid(Guid receiverGuid)
		{
			if (receiverGuid == Guid.Empty)
			{
				return _mainPlayer;
			}

			return Decorators.Find((x) => x.PlayerGuid == receiverGuid);
		}

		public void AddNewPlayer(NewPlayerConnectSendingData sendingData)
		{
			Decorators[sendingData.Index].SetPlayerData(sendingData.Guid, new PlayerData(sendingData.Name, sendingData.Money, 0, string.Empty, 0, sendingData.Index, false, false));
		}

		public void RemovePlayer(Guid player)
		{
			ConsoleUiDecorator decorator = Decorators.FirstOrDefault((x) => x.PlayerGuid == player);
			if (decorator != null)
			{
				decorator.SetEmpty();
			}
		}

		public void HighlightWinners(List<(Guid, int, string)> winners)
		{
			foreach (var winner in winners)
			{
				ConsoleUiDecorator decorator = Decorators.FirstOrDefault((x) => x.PlayerGuid == winner.Item1);
				if (decorator != null)
				{
					decorator.SetWinner(winner.Item2, winner.Item3);
				}
			}
		}
	}
}
