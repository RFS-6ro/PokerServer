using System;
using System.Collections.Generic;
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

		public ConsoleUiDecorator AddNewPlayer(NewPlayerConnectSendingData sendingData)
		{
			throw new NotImplementedException();
		}

		internal ConsoleUiDecorator RemovePlayer(Guid player)
		{
			throw new NotImplementedException();
		}
	}
}
