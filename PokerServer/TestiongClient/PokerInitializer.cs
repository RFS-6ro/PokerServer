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

		public List<ServerPlayer> CurrentPlayers = new(MaxPlayers);
		public List<ConsoleUiDecorator> Decorators = new(MaxPlayers);

		public TableViewModel Table { get; }

		public PokerInitializer()
		{
			IStaticInstance<PokerInitializer>.Instance = this;
			Table = new TableViewModel(30);
			for (int i = 0; i < MaxPlayers; i++)
			{
				CurrentPlayers.Add(null);
				ConsoleUiDecorator decorator = new ConsoleUiDecorator();
				decorator.DrawGameBox((6 * i) + 3, 30, 1);
				Decorators.Add(decorator);
			}
		}

		public void SetCurrentGameState(CurrentGameStateSendingData sendingData)
		{
			var keys = sendingData.Datas.Select((x) => x.Item1).ToList();

			int realPlayerIndex = keys.IndexOf(IStaticInstance<Client_Lobby>.Instance.Id);

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
				// ??
				//return main player
			}
			throw new NotImplementedException();
		}

		public void AddNewPlayer(NewPlayerConnectSendingData sendingData)
		{
			Decorators[sendingData.Index].SetPlayerData(sendingData.Guid, new PlayerData(sendingData.Name, sendingData.Money, 0, string.Empty, 0, sendingData.Index, false, false));
		}

		public ConsoleUiDecorator RemovePlayer(Guid player)
		{
			throw new NotImplementedException();
		}
	}
}
