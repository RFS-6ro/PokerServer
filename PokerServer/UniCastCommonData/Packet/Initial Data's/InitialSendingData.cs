using System;
using System.Collections.Generic;
using System.Linq;
using UniCastCommonData.Handlers;
using UniCastCommonData.Handlers.Convert;

namespace UniCastCommonData.Packet.InitialDatas
{
	public class InitialSendingData : IByteArrayConvertable
	{
		public ActorType ActorType { get; set; }

		public int Action { get; set; }

		public Guid SenderGuid { get; set; }

		public Guid ReceiverGuid { get; set; }

		public InitialSendingData(byte[] data)
		{
			ActorType = (ActorType)data.ToInt32(4);
			Action = data.ToInt32(8);
			SenderGuid = data.ToGuid(12);
			ReceiverGuid = data.ToGuid(28);
		}

		public InitialSendingData(Guid receiverGuid, Guid senderGuid, ActorType actorType, int action)
		{
			ActorType = actorType;
			Action = action;
			SenderGuid = senderGuid;
			ReceiverGuid = receiverGuid;
		}

		public virtual byte[] GetRawBytes()
		{
			byte[] data = new byte[40];

			((int)ActorType).ToByteArray().CopyTo(data, 0);
			Action.ToByteArray().CopyTo(data, 4);
			SenderGuid.ToByteArray().CopyTo(data, 8);
			ReceiverGuid.ToByteArray().CopyTo(data, 24);

			return data;
		}
	}

	public struct Tuple<T1, T2>
	{
		private T1 _item1;
		public T1 Item1 => _item1;

		private T2 _item2;
		public T2 Item2 => _item2;

		public Tuple(T1 item1, T2 item2)
		{
			_item1 = item1;
			_item2 = item2;
		}
	}

	public struct Tuple<T1, T2, T3>
	{
		private T1 _item1;
		public T1 Item1 => _item1;

		private T2 _item2;
		public T2 Item2 => _item2;

		private T3 _item3;
		public T3 Item3 => _item3;

		public Tuple(T1 item1, T2 item2, T3 item3)
		{
			_item1 = item1;
			_item2 = item2;
			_item3 = item3;
		}
	}

	public struct Tuple<T1, T2, T3, T4>
	{
		private T1 _item1;
		public T1 Item1 => _item1;

		private T2 _item2;
		public T2 Item2 => _item2;

		private T3 _item3;
		public T3 Item3 => _item3;

		private T4 _item4;
		public T4 Item4 => _item4;

		public Tuple(T1 item1, T2 item2, T3 item3, T4 item4)
		{
			_item1 = item1;
			_item2 = item2;
			_item3 = item3;
			_item4 = item4;
		}
	}

	#region from lobby
	public class PlayerData : IByteArrayConvertable
	{
		public int Money { get; set; }
		public int Index { get; set; }
		public int Bet { get; set; }
		public int LastPlayerActionAmount { get; set; }
		public string Name { get; set; }
		public string LastPlayerAction { get; set; }
		public bool InGame { get; set; }
		public bool IsDealer { get; set; }

		public PlayerData(string name, int money, int bet, string lastPlayerAction, int lastPlayerActionAmount, int index, bool inGame, bool isDealer)
		{
			InGame = inGame;
			IsDealer = isDealer;
			Name = name;
			Money = money;
			Bet = bet;
			Index = index;
			LastPlayerAction = lastPlayerAction;
			LastPlayerActionAmount = lastPlayerActionAmount;
		}

		public byte[] GetRawBytes()
		{
			List<byte> data = new List<byte>();

			data.AddRange(Money.ToByteArray());//4
			data.AddRange(Bet.ToByteArray());//4
			data.AddRange(LastPlayerActionAmount.ToByteArray());//4
			data.AddRange(Index.ToByteArray());//4
			data.AddRange(InGame.ToByteArray());//1
			data.AddRange(IsDealer.ToByteArray());//1


			if (Name == string.Empty)
			{
				Name = "Empty";
			}

			if (Name.Length > 20)
			{
				Name.Take(20);
			}
			else
			{
				Name += new string(' ', 20 - Name.Length);
			}
			data.AddRange(Name.ToByteArray()); //20

			if (LastPlayerAction.Length > 20)
			{
				LastPlayerAction.Take(20);
			}
			else
			{
				LastPlayerAction += new string(' ', 20 - LastPlayerAction.Length);
			}

			data.AddRange(LastPlayerAction.ToByteArray()); //20

			return data.ToArray();
		}
	}

	public class CurrentGameStateSendingData : InitialSendingData
	{
		public List<Tuple<Guid, PlayerData>> Datas = new List<Tuple<Guid, PlayerData>>();

		public CurrentGameStateSendingData(byte[] data) : base(data)
		{
			int length = data.ToInt32(44);

			for (int i = 0; i < length; i++)
			{
				Guid key = data.ToGuid(48 + 82 * i);//16
				int money = data.ToInt32(64 + 82 * i);//20
				int bet = data.ToInt32(68 + 82 * i);//24
				int lastPlayerActionAmount = data.ToInt32(72 + 82 * i);//28
				int index = data.ToInt32(76 + 82 * i);//32
				bool inGame = data.ToBoolean(80 + 82 * i);//33
				bool isDealer = data.ToBoolean(81 + 82 * i);//34
				string name = data.ToString(82 + 82 * i);//58
				string lastPlayerAction = data.ToString(106 + 82 * i);//82
				Datas.Add(new Tuple<Guid, PlayerData>(key, new PlayerData(name, money, bet, lastPlayerAction, lastPlayerActionAmount, index, inGame, isDealer)));
			}
		}

		public CurrentGameStateSendingData(List<Tuple<Guid, PlayerData>> datas, Guid receiverGuid, Guid senderGuid, ActorType actorType, int action) : base(receiverGuid, senderGuid, actorType, action)
		{
			Datas = datas;
		}

		public override byte[] GetRawBytes()
		{
			List<byte> data = new List<byte>();
			data.AddRange(base.GetRawBytes());

			data.AddRange(Datas.Count.ToByteArray());

			foreach (var playerData in Datas)
			{
				data.AddRange(playerData.Item1.ToByteArray());
				data.AddRange(playerData.Item2.GetRawBytes());
			}

			return data.ToArray();
		}
	}

	public class NewPlayerConnectSendingData : InitialSendingData
	{
		protected int _index;
		public int Index => _index;

		protected int _money;
		public int Money => _money;

		protected string _name;
		public string Name => _name;

		protected Guid _guid;
		public Guid Guid => _guid;

		public NewPlayerConnectSendingData(byte[] data) : base(data)
		{
			_guid = data.ToGuid(40 + 4);
			_index = data.ToInt32(56 + 4);
			_name = data.ToString(60 + 4);
			_money = data.ToInt32(64 + 4 + _name.Length);
		}

		public NewPlayerConnectSendingData(int money, string name, int index, Guid guid, Guid receiverGuid, Guid senderGuid, ActorType actorType, int action) : base(receiverGuid, senderGuid, actorType, action)
		{
			_name = name;
			_money = money;
			_index = index;
			_guid = guid;
			//TODO: add image
		}

		public override byte[] GetRawBytes()
		{
			List<byte> data = new List<byte>();
			data.AddRange(base.GetRawBytes());
			data.AddRange(_guid.ToByteArray());
			data.AddRange(_index.ToByteArray());
			data.AddRange(_name.ToByteArray());
			data.AddRange(_money.ToByteArray());
			return data.ToArray();
		}
	}

	public class StartGameSendingData : InitialSendingData
	{
		private int _startMoney;
		public int StartMoney => _startMoney;

		public StartGameSendingData(byte[] data) : base(data)
		{
			_startMoney = data.ToInt32(40 + 4);
		}

		public StartGameSendingData(int startMoney, Guid receiverGuid, Guid senderGuid, ActorType actorType, int action) : base(receiverGuid, senderGuid, actorType, action)
		{
			_startMoney = startMoney;
		}

		public override byte[] GetRawBytes()
		{
			List<byte> data = new List<byte>();
			data.AddRange(base.GetRawBytes());
			data.AddRange(_startMoney.ToByteArray());
			return data.ToArray();
		}
	}

	public class StartHandSendingData : InitialSendingData
	{
		private int _handNumber;
		public int HandNumber => _handNumber;

		private int _money;
		public int Money => _money;

		private int _smallBlind;
		public int SmallBlind => _smallBlind;

		private string _firstPlayerName;
		public string FirstPlayerName => _firstPlayerName;

		public StartHandSendingData(byte[] data) : base(data)
		{
			_handNumber = data.ToInt32(40 + 4);
			_money = data.ToInt32(44 + 4);
			_smallBlind = data.ToInt32(48 + 4);
			_firstPlayerName = data.ToString(52 + 4);
		}

		public StartHandSendingData(int handNumber, int money, int smallBlind, string firstPlayerName, Guid receiverGuid, Guid senderGuid, ActorType actorType, int action) : base(receiverGuid, senderGuid, actorType, action)
		{
			_handNumber = handNumber;
			_money = money;
			_smallBlind = smallBlind;
			_firstPlayerName = firstPlayerName;
		}

		public override byte[] GetRawBytes()
		{
			List<byte> data = new List<byte>();
			data.AddRange(base.GetRawBytes());
			data.AddRange(_handNumber.ToByteArray());
			data.AddRange(_money.ToByteArray());
			data.AddRange(_smallBlind.ToByteArray());
			data.AddRange(_firstPlayerName.ToByteArray());

			return data.ToArray();
		}
	}

	public class StartRoundSendingData : InitialSendingData
	{
		private int _pot;
		public int Pot => _pot;

		private int _money;
		public int Money => _money;

		public StartRoundSendingData(byte[] data) : base(data)
		{
			_pot = data.ToInt32(40 + 4);
			_money = data.ToInt32(44 + 4);
		}

		public StartRoundSendingData(int pot, int money, Guid receiverGuid, Guid senderGuid, ActorType actorType, int action) : base(receiverGuid, senderGuid, actorType, action)
		{
			_pot = pot;
			_money = money;
		}

		public override byte[] GetRawBytes()
		{
			List<byte> data = new List<byte>();
			data.AddRange(base.GetRawBytes());
			data.AddRange(_pot.ToByteArray());
			data.AddRange(_money.ToByteArray());

			return data.ToArray();
		}
	}

	public class StartTurnSendingData : InitialSendingData
	{
		protected Guid _player;
		public Guid Player => _player;

		private int _time;
		public int Time => _time;

		private int _gameRoundType;
		public int GameRoundType => _gameRoundType;

		private int _smallBlind;
		public int SmallBlind => _smallBlind;

		private int _money;
		public int Money => _money;

		private int _pot;
		public int Pot => _pot;

		private int _currentRoundBet;
		public int CurrentRoundBet => _currentRoundBet;

		private int _maxMoneyPerPlayer;
		public int MaxMoneyPerPlayer => _maxMoneyPerPlayer;

		private int _minRaise;
		public int MinRaise => _minRaise;

		private int _myMoneyInTheRound;
		public int MyMoneyInTheRound => _myMoneyInTheRound;

		private int _moneyToCall;
		public int MoneyToCall => _moneyToCall;

		private bool _isAllIn;
		public bool IsAllIn => _isAllIn;

		private bool _canRaise;
		public bool CanRaise => _canRaise;

		private bool _canCheck;
		public bool CanCheck => _canCheck;

		public StartTurnSendingData(byte[] data) : base(data)
		{
			_time = data.ToInt32(40 + 4);
			_gameRoundType = data.ToInt32(44 + 4);
			_smallBlind = data.ToInt32(48 + 4);
			_money = data.ToInt32(52 + 4);
			_pot = data.ToInt32(56 + 4);
			_currentRoundBet = data.ToInt32(60 + 4);
			_maxMoneyPerPlayer = data.ToInt32(64 + 4);
			_minRaise = data.ToInt32(68 + 4);
			_myMoneyInTheRound = data.ToInt32(72 + 4);
			_moneyToCall = data.ToInt32(76 + 4);
			_isAllIn = data.ToBoolean(80 + 4);
			_canRaise = data.ToBoolean(81 + 4);
			_canCheck = data.ToBoolean(82 + 4);
			_player = data.ToGuid(83 + 4);
		}

		public StartTurnSendingData(Guid player, int time, int gameRoundType, int smallBlind, int money, int pot, int currentRoundBet, int maxMoneyPerPlayer, int minRaise, int myMoneyInTheRound, int moneyToCall, bool isAllIn, bool canRaise, bool canCheck, Guid receiverGuid, Guid senderGuid, ActorType actorType, int action) : base(receiverGuid, senderGuid, actorType, action)
		{
			_player = player;
			_time = time;
			_gameRoundType = gameRoundType;
			_smallBlind = smallBlind;
			_money = money;
			_pot = pot;
			_currentRoundBet = currentRoundBet;
			_maxMoneyPerPlayer = maxMoneyPerPlayer;
			_minRaise = minRaise;
			_myMoneyInTheRound = myMoneyInTheRound;
			_moneyToCall = moneyToCall;
			_isAllIn = isAllIn;
			_canRaise = canRaise;
			_canCheck = canCheck;
		}

		public override byte[] GetRawBytes()
		{
			List<byte> data = new List<byte>();
			data.AddRange(base.GetRawBytes());

			data.AddRange(_time.ToByteArray());
			data.AddRange(_gameRoundType.ToByteArray());
			data.AddRange(_smallBlind.ToByteArray());
			data.AddRange(_money.ToByteArray());
			data.AddRange(_pot.ToByteArray());
			data.AddRange(_currentRoundBet.ToByteArray());
			data.AddRange(_maxMoneyPerPlayer.ToByteArray());
			data.AddRange(_minRaise.ToByteArray());
			data.AddRange(_myMoneyInTheRound.ToByteArray());
			data.AddRange(_moneyToCall.ToByteArray());
			data.AddRange(_isAllIn.ToByteArray());
			data.AddRange(_canRaise.ToByteArray());
			data.AddRange(_canCheck.ToByteArray());
			data.AddRange(_player.ToByteArray());

			return data.ToArray();
		}
	}

	public class DealCardsToPlayerSendingData : InitialSendingData
	{
		private int _cardType1;
		public int CardType1 => _cardType1;

		private int _cardSuit1;
		public int CardSuit1 => _cardSuit1;

		private int _cardType2;
		public int CardType2 => _cardType2;

		private int _cardSuit2;
		public int CardSuit2 => _cardSuit2;

		public DealCardsToPlayerSendingData(byte[] data) : base(data)
		{
			_cardType1 = data.ToInt32(40 + 4);
			_cardSuit1 = data.ToInt32(44 + 4);
			_cardType2 = data.ToInt32(48 + 4);
			_cardSuit2 = data.ToInt32(52 + 4);
		}

		public DealCardsToPlayerSendingData(int cardType1, int cardSuit1, int cardType2, int cardSuit2, Guid receiverGuid, Guid senderGuid, ActorType actorType, int action) : base(receiverGuid, senderGuid, actorType, action)
		{
			_cardType1 = cardType1;
			_cardSuit1 = cardSuit1;
			_cardType2 = cardType2;
			_cardSuit2 = cardSuit2;
		}

		public override byte[] GetRawBytes()
		{
			List<byte> data = new List<byte>();
			data.AddRange(base.GetRawBytes());
			data.AddRange(_cardType1.ToByteArray());
			data.AddRange(_cardSuit1.ToByteArray());
			data.AddRange(_cardType2.ToByteArray());
			data.AddRange(_cardSuit2.ToByteArray());
			return data.ToArray();
		}
	}

	public class DealCardsToTableSendingData : InitialSendingData
	{
		public List<Tuple<int, int, int>> Cards = new List<Tuple<int, int, int>>();

		public DealCardsToTableSendingData(byte[] data) : base(data)
		{
			int length = data.ToInt32(40 + 4);

			for (int i = 0; i < length; i++)
			{
				Cards.Add(new Tuple<int, int, int>(data.ToInt32(44 + 4 + 12 * i), data.ToInt32(48 + 4 + 12 * i), data.ToInt32(52 + 4 + 12 * i)));
			}
		}

		public DealCardsToTableSendingData(List<Tuple<int, int, int>> cards, Guid receiverGuid, Guid senderGuid, ActorType actorType, int action) : base(receiverGuid, senderGuid, actorType, action)
		{
			Cards = cards;
		}

		public override byte[] GetRawBytes()
		{
			List<byte> data = new List<byte>();
			data.AddRange(base.GetRawBytes());

			data.AddRange(Cards.Count.ToByteArray());

			for (int i = 0; i < Cards.Count; i++)
			{
				data.AddRange(Cards[i].Item1.ToByteArray());
				data.AddRange(Cards[i].Item2.ToByteArray());
				data.AddRange(Cards[i].Item3.ToByteArray());
			}

			return data.ToArray();
		}
	}

	public class WinnersSendingData : InitialSendingData
	{
		public List<Tuple<Guid, int, string>> Winners = new List<Tuple<Guid, int, string>>();

		public WinnersSendingData(byte[] data) : base(data)
		{
			int length = data.ToInt32(40 + 4);

			for (int i = 0; i < length; i++)
			{
				Winners.Add(new Tuple<Guid, int, string>(
					data.ToGuid(44 + 4 + 37 * i),
					data.ToInt32(60 + 4 + 37 * i),
					data.ToString(64 + 4 + 37 * i)
				));
			}
		}

		public WinnersSendingData(List<Tuple<Guid, int, string>> winners, Guid receiverGuid, Guid senderGuid, ActorType actorType, int action) : base(receiverGuid, senderGuid, actorType, action)
		{
			Winners = winners;
		}

		public override byte[] GetRawBytes()
		{
			List<byte> data = new List<byte>();
			data.AddRange(base.GetRawBytes());

			data.AddRange(Winners.Count.ToByteArray());

			foreach (var winner in Winners)
			{
				data.AddRange(winner.Item1.ToByteArray());
				data.AddRange(winner.Item2.ToByteArray());
				data.AddRange(winner.Item3.ToByteArray());
			}

			return data.ToArray();
		}
	}

	public class DealerButtonSendingData : InitialSendingData
	{
		protected Guid _dealer;
		public Guid Dealer => _dealer;

		public DealerButtonSendingData(byte[] data) : base(data)
		{
			_dealer = data.ToGuid(40 + 4);
		}

		public DealerButtonSendingData(Guid dealer, Guid receiverGuid, Guid senderGuid, ActorType actorType, int action) : base(receiverGuid, senderGuid, actorType, action)
		{
			_dealer = dealer;
		}

		public override byte[] GetRawBytes()
		{
			List<byte> data = new List<byte>();
			data.AddRange(base.GetRawBytes());
			data.AddRange(_dealer.ToByteArray());
			return data.ToArray();
		}
	}

	public class DisconnectSendingData : InitialSendingData
	{
		protected Guid _player;
		public Guid Player => _player;

		public DisconnectSendingData(byte[] data) : base(data)
		{
			_player = data.ToGuid(40 + 4);
		}

		public DisconnectSendingData(Guid player, Guid receiverGuid, Guid senderGuid, ActorType actorType, int action) : base(receiverGuid, senderGuid, actorType, action)
		{
			_player = player;
		}

		public override byte[] GetRawBytes()
		{
			List<byte> data = new List<byte>();
			data.AddRange(base.GetRawBytes());
			data.AddRange(_player.ToByteArray());
			return data.ToArray();
		}
	}

	public class UpdatePotSendingData : InitialSendingData
	{
		private int _pot;
		public int Pot => _pot;

		public UpdatePotSendingData(byte[] data) : base(data)
		{
			_pot = data.ToInt32(40 + 4);
		}

		public UpdatePotSendingData(int pot, Guid receiverGuid, Guid senderGuid, ActorType actorType, int action) : base(receiverGuid, senderGuid, actorType, action)
		{
			_pot = pot;
		}

		public override byte[] GetRawBytes()
		{
			List<byte> data = new List<byte>();
			data.AddRange(base.GetRawBytes());
			data.AddRange(_pot.ToByteArray());
			return data.ToArray();
		}
	}

	public class UpdatePlayersMoneySendingData : InitialSendingData
	{
		public Dictionary<Guid, int> Moneys = new Dictionary<Guid, int>();

		public UpdatePlayersMoneySendingData(byte[] data) : base(data)
		{
			int length = data.ToInt32(40 + 4);

			for (int i = 0; i < length; i++)
			{
				Guid key = data.ToGuid(44 + 4 + 20 * i);
				int value = data.ToInt32(60 + 4 + 20 * i);
				Moneys.Add(key, value);
			}
		}

		public UpdatePlayersMoneySendingData(Dictionary<Guid, int> moneys, Guid receiverGuid, Guid senderGuid, ActorType actorType, int action) : base(receiverGuid, senderGuid, actorType, action)
		{
			Moneys = moneys;
		}

		public override byte[] GetRawBytes()
		{
			List<byte> data = new List<byte>();
			data.AddRange(base.GetRawBytes());

			data.AddRange(Moneys.Count.ToByteArray());

			foreach (var money in Moneys)
			{
				data.AddRange(money.Key.ToByteArray());
				data.AddRange(money.Value.ToByteArray());
			}

			return data.ToArray();
		}
	}

	public class UpdateTimerSendingData : InitialSendingData
	{
		private Guid _player;
		public Guid Player => _player;

		private int _milliseconds;
		public int Milliseconds => _milliseconds;

		public UpdateTimerSendingData(byte[] data) : base(data)
		{
			_player = data.ToGuid(40 + 4);
			_milliseconds = data.ToInt32(56 + 4);
		}

		public UpdateTimerSendingData(Guid player, int milliseconds, Guid receiverGuid, Guid senderGuid, ActorType actorType, int action) : base(receiverGuid, senderGuid, actorType, action)
		{
			_player = player;
			_milliseconds = milliseconds;
		}

		public override byte[] GetRawBytes()
		{
			List<byte> data = new List<byte>();
			data.AddRange(base.GetRawBytes());
			data.AddRange(_player.ToByteArray());
			data.AddRange(_milliseconds.ToByteArray());
			return data.ToArray();
		}
	}

	public class PlayerTurnSendingData : InitialSendingData
	{
		protected Guid _player;
		public Guid Player => _player;

		private int _lastPlayerActionAmount;
		public int LastPlayerActionAmount => _lastPlayerActionAmount;

		private int _moneyLeft;
		public int MoneyLeft => _moneyLeft;

		private int _moneyInTheRound;
		public int MoneyInTheRound => _moneyInTheRound;

		private int _moneyToCall;
		public int MoneyToCall => _moneyToCall;

		private int _minRaise;
		public int MinRaise => _minRaise;

		private int _maxBet;
		public int MaxBet => _maxBet;

		private string _lastPlayerAction;
		public string LastPlayerAction => _lastPlayerAction;

		public PlayerTurnSendingData(byte[] data) : base(data)
		{
			_player = data.ToGuid(40 + 4);
			_lastPlayerActionAmount = data.ToInt32(56 + 4);
			_moneyLeft = data.ToInt32(60 + 4);
			_moneyInTheRound = data.ToInt32(64 + 4);
			_moneyToCall = data.ToInt32(68 + 4);
			_minRaise = data.ToInt32(72 + 4);
			_maxBet = data.ToInt32(76 + 4);
			_lastPlayerAction = data.ToString(80 + 4);
		}

		public PlayerTurnSendingData(Guid playerGuid, int moneyLeft, int moneyInTheRound, int moneyToCall, int lastPlayerActionAmount, int minRaise, int maxBet, string lastPlayerAction, Guid receiverGuid, Guid senderGuid, ActorType actorType, int action) : base(receiverGuid, senderGuid, actorType, action)
		{
			_moneyInTheRound = moneyInTheRound;
			_moneyToCall = moneyToCall;
			_moneyLeft = moneyLeft;
			_player = playerGuid;
			_lastPlayerAction = lastPlayerAction;
			_lastPlayerActionAmount = lastPlayerActionAmount;
			_minRaise = minRaise;
			_maxBet = maxBet;
		}

		public override byte[] GetRawBytes()
		{
			List<byte> data = new List<byte>();
			data.AddRange(base.GetRawBytes());
			data.AddRange(_player.ToByteArray());
			data.AddRange(_lastPlayerActionAmount.ToByteArray());
			data.AddRange(_moneyLeft.ToByteArray());
			data.AddRange(_moneyInTheRound.ToByteArray());
			data.AddRange(_moneyToCall.ToByteArray());
			data.AddRange(_minRaise.ToByteArray());
			data.AddRange(_maxBet.ToByteArray());
			data.AddRange(_lastPlayerAction.ToByteArray());
			return data.ToArray();
		}
	}

	public class OpponentCardsSendingData : InitialSendingData
	{
		public Dictionary<Guid, Tuple<int, int, int, int>> Cards = new Dictionary<Guid, Tuple<int, int, int, int>>();

		public OpponentCardsSendingData(byte[] data) : base(data)
		{
			int length = data.ToInt32(40 + 4);

			for (int i = 0; i < length; i++)
			{
				Guid key = data.ToGuid(44 + 4 + 32 * i);
				int type1 = data.ToInt32(60 + 4 + 32 * i);
				int suit1 = data.ToInt32(64 + 4 + 32 * i);
				int type2 = data.ToInt32(68 + 4 + 32 * i);
				int suit2 = data.ToInt32(72 + 4 + 32 * i);
				Cards.Add(key, new Tuple<int, int, int, int>(type1, suit1, type2, suit2));
			}
		}

		public OpponentCardsSendingData(Dictionary<Guid, Tuple<int, int, int, int>> cards, Guid receiverGuid, Guid senderGuid, ActorType actorType, int action) : base(receiverGuid, senderGuid, actorType, action)
		{
			Cards = cards;
		}

		public override byte[] GetRawBytes()
		{
			List<byte> data = new List<byte>();
			data.AddRange(base.GetRawBytes());

			data.AddRange(Cards.Count.ToByteArray());

			foreach (var player in Cards)
			{
				data.AddRange(player.Key.ToByteArray());
				data.AddRange(player.Value.Item1.ToByteArray());
				data.AddRange(player.Value.Item2.ToByteArray());
				data.AddRange(player.Value.Item3.ToByteArray());
				data.AddRange(player.Value.Item4.ToByteArray());
			}

			return data.ToArray();
		}
	}

	public class EndTurnSendingData : InitialSendingData
	{
		protected Guid _player;
		public Guid Player => _player;

		public EndTurnSendingData(byte[] data) : base(data)
		{
			_player = data.ToGuid(40 + 4);
		}

		public EndTurnSendingData(Guid playerGuid, Guid receiverGuid, Guid senderGuid, ActorType actorType, int action) : base(receiverGuid, senderGuid, actorType, action)
		{
			_player = playerGuid;
		}

		public override byte[] GetRawBytes()
		{
			List<byte> data = new List<byte>();
			data.AddRange(base.GetRawBytes());
			data.AddRange(_player.ToByteArray());
			return data.ToArray();
		}
	}

	public class EndRoundSendingData : InitialSendingData
	{
		public EndRoundSendingData(byte[] data) : base(data)
		{
		}

		public EndRoundSendingData(Guid receiverGuid, Guid senderGuid, ActorType actorType, int action) : base(receiverGuid, senderGuid, actorType, action)
		{
		}

		public override byte[] GetRawBytes()
		{
			List<byte> data = new List<byte>();
			data.AddRange(base.GetRawBytes());
			return data.ToArray();
		}
	}

	public class EndHandSendingData : InitialSendingData
	{
		public EndHandSendingData(byte[] data) : base(data)
		{
		}

		public EndHandSendingData(Guid receiverGuid, Guid senderGuid, ActorType actorType, int action) : base(receiverGuid, senderGuid, actorType, action)
		{
		}

		public override byte[] GetRawBytes()
		{
			List<byte> data = new List<byte>();
			data.AddRange(base.GetRawBytes());
			return data.ToArray();
		}
	}

	public class EndGameSendingData : InitialSendingData
	{
		protected Guid _winner;
		public Guid Winner => _winner;

		public EndGameSendingData(byte[] data) : base(data)
		{
			_winner = data.ToGuid(40 + 4);
		}

		public EndGameSendingData(Guid winner, Guid receiverGuid, Guid senderGuid, ActorType actorType, int action) : base(receiverGuid, senderGuid, actorType, action)
		{
			_winner = winner;
		}

		public override byte[] GetRawBytes()
		{
			List<byte> data = new List<byte>();
			data.AddRange(base.GetRawBytes());
			data.AddRange(_winner.ToByteArray());
			return data.ToArray();
		}
	}

	public class ClearCardsSendingData : InitialSendingData
	{
		private Guid _cardKeeper;
		public Guid CardKeeper => _cardKeeper;

		public ClearCardsSendingData(byte[] data) : base(data)
		{
			_cardKeeper = data.ToGuid(40 + 4);
		}

		public ClearCardsSendingData(Guid cardKeeper, Guid receiverGuid, Guid senderGuid, ActorType actorType, int action) : base(receiverGuid, senderGuid, actorType, action)
		{
			_cardKeeper = cardKeeper;
		}

		public override byte[] GetRawBytes()
		{
			List<byte> data = new List<byte>();
			data.AddRange(base.GetRawBytes());
			data.AddRange(_cardKeeper.ToByteArray());
			return data.ToArray();
		}
	}

	public class PlayerDisconnectSendingData : InitialSendingData
	{
		private Guid _player;
		public Guid Player => _player;

		public PlayerDisconnectSendingData(byte[] data) : base(data)
		{
			_player = data.ToGuid(40 + 4);
		}

		public PlayerDisconnectSendingData(Guid player, Guid receiverGuid, Guid senderGuid, ActorType actorType, int action) : base(receiverGuid, senderGuid, actorType, action)
		{
			_player = player;
		}

		public override byte[] GetRawBytes()
		{
			List<byte> data = new List<byte>();
			data.AddRange(base.GetRawBytes());
			data.AddRange(_player.ToByteArray());
			return data.ToArray();
		}
	}

	public class PlayerInputSendingData : InitialSendingData
	{
		private byte[] _inputData;
		private int _inputType;
		private int _inputAmount;
		public int InputType => _inputType;
		public int InputAmount => _inputAmount;

		public PlayerInputSendingData(byte[] data) : base(data)
		{
			_inputData = data.Where((b, i) => i >= 40).ToArray();
			_inputType = _inputData.ToInt32(0);
			_inputAmount = _inputData.ToInt32(4);
		}

		public PlayerInputSendingData(byte[] inputData, Guid receiverGuid, Guid senderGuid, ActorType actorType, int action) : base(receiverGuid, senderGuid, actorType, action)
		{
			_inputData = inputData;
		}

		public override byte[] GetRawBytes()
		{
			List<byte> data = new List<byte>();
			data.AddRange(base.GetRawBytes());
			data.AddRange(_inputData);
			return data.ToArray();
		}
	}
	#endregion

}
