using LobbyServer.Client;
using LobbyServer.Client.Handlers;
using LobbyServer.pokerlogic.pokermodel.UI;
using PokerSynchronisation;
using ServerDLL;
using System;
using System.Threading.Tasks;
using UniCastCommonData;
using UniCastCommonData.Network.MessageHandlers;
using UniCastCommonData.Packet.InitialDatas;

namespace LobbyServer.pokerlogic.pokermodel.Players
{
	/*

		StaticLogger.Print($"Server Player + {Guid}", $"");
		StaticLogger.Print($"Server Player + {Guid}",
			new string[]
			{
				$"",
			}
		);

	 */
	public class ServerPlayer : BasePlayer
	{
		public int Row { get; protected set; }

		public Guid Guid { get; }

		public override string Name { get; }

		public override int BuyIn { get; }

		private int _playerRaiseAmount = -1;
		private TurnType _turnType = TurnType.None;

		private Lobby_Client_Server Server => Lobby_Client_Server.Instance;
		private SessionSender<Lobby_Client_Server> Sender => Lobby_Client_Server.Instance.SendHandler;

		public ServerPlayer(Guid guid, string name, int buyIn = -1)
		{
			Name = name;
			Guid = guid;
			BuyIn = buyIn;
		}

		public void SetRow(int row)
		{
			Row = row;
		}

		public override PlayerAction PostingBlind(IPostingBlindContext context)
		{
			StaticLogger.Print($"Server Player + {Guid}", $"posting blind {context.BlindAction.Money}");
			return context.BlindAction;
		}

		public async override Task<PlayerAction> GetTurn(IGetTurnContext context)
		{
			int passedTime = 0;

			StaticLogger.Print($"Server Player + {Guid}",
				new string[]
				{
					$"awaiting for turn",
					$"timer value = {context.TimeForTurn}",
					$"CanRaise = {context.CanRaise}",
					$"CanCheck = {context.CanCheck}",
					$"CurrentMaxBet = {context.CurrentMaxBet}",
					$"CurrentPot = {context.CurrentPot}",
					$"IsAllIn = {context.IsAllIn}",
					$"MainPot = {context.MainPot.AmountOfMoney}",
					$"MinRaise = {context.MinRaise}",
					$"MoneyLeft = {context.MoneyLeft}",
					$"MoneyToCall = {context.MoneyToCall}",
					$"MyMoneyInTheRound = {context.MyMoneyInTheRound}",
					$"RoundType = {context.RoundType}",
					$"SmallBlind = {context.SmallBlind}",
				}
			);

			while (passedTime < context.TimeForTurn)
			{
				//RECEIVE: player turn
				PlayerAction action = null;
				switch (_turnType)
				{
				case TurnType.None:
					break;
				case TurnType.Call:
					action = PlayerAction.CheckOrCall();
					break;
				case TurnType.Raise:
					if (!context.CanRaise)
					{
						break;
					}
					int amount = RaiseAmount(context.MoneyLeft, context.MinRaise, context.MoneyToCall, context.MyMoneyInTheRound);
					action = PlayerAction.Raise(amount);
					break;
				case TurnType.Fold:
					action = PlayerAction.Fold();
					break;
				case TurnType.AllIn:
					if (!context.CanRaise)
					{
						break;
					}

					action = context.MoneyLeft > 0
								 ? PlayerAction.Raise(context.MoneyLeft - context.MoneyToCall)
								 : PlayerAction.CheckOrCall();
					break;
				}

				if (action != null)
				{
					_playerRaiseAmount = -1;
					_turnType = TurnType.None;

					StaticLogger.Print($"Server Player + {Guid}", $"result valid action = {action}");
					return action;
				}

				await Task.Delay(100);
				passedTime += 100;

				Sender.SendAsync(new UpdateTimerSendingData(
									 Guid,
									 context.TimeForTurn - passedTime,
									 Guid,
									 Server.Id,
									 Server.ServerType,
									 (int)lobbyTOclient.UpdateTimer),
								 null);
			}

			StaticLogger.Print($"Server Player + {Guid}", $"Turn timeout");
			return PlayerAction.Fold();
		}

		public void SetPlayerTurn(int type, int amount)
		{
			//RECEIVE:
			_turnType = (TurnType)type;
			_playerRaiseAmount = amount;
			StaticLogger.Print($"Server Player + {Guid}", $"player turn received: [{_turnType}, {_playerRaiseAmount}]");
		}

		private int RaiseAmount(int moneyLeft, int minRaise, int moneyToCall, int myMoneyInTheRound)
		{
			int amount;
			var wholeMinRaise = minRaise + myMoneyInTheRound + moneyToCall;
			if (wholeMinRaise >= moneyLeft + myMoneyInTheRound)
			{
				// Instant All-In
				amount = moneyLeft - moneyToCall;
			}
			else
			{
				int result = _playerRaiseAmount;

				if (result < wholeMinRaise)
				{
					amount = minRaise;
				}
				else if (result >= moneyLeft + myMoneyInTheRound)
				{
					// Raise All-in
					amount = moneyLeft - moneyToCall;
				}
				else
				{
					amount = result - (myMoneyInTheRound + moneyToCall);
				}
			}

			_playerRaiseAmount = -1;

			StaticLogger.Print($"Server Player + {Guid}", $"valid raise amount = {amount}");
			return amount;
		}
	}
}
