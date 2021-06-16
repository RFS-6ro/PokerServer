using System;
using System.Threading.Tasks;
using LobbyServer.Client;
using LobbyServer.Client.Handlers;
using LobbyServer.pokerlogic.pokermodel.UI;
using UniCastCommonData;
using UniCastCommonData.Network.MessageHandlers;
using UniCastCommonData.Packet.InitialDatas;

namespace LobbyServer.pokerlogic.pokermodel.Players
{
	public class ServerPlayer : BasePlayer
	{
		public int Row { get; protected set; }

		public Guid Guid { get; }

		public override string Name { get; }

		public override int BuyIn { get; }

		private Lobby_Client_Server Server => IStaticInstance<Lobby_Client_Server>.Instance;
		private SessionSender<Lobby_Client_Server> Sender => IStaticInstance<Lobby_Client_Server>.Instance.SendHandler;

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
			return context.BlindAction;
		}

		public async override Task<PlayerAction> GetTurn(IGetTurnContext context)
		{
			int passedTime = 0;

			while (passedTime < context.TimeForTurn)
			{
				//TODORECEIVE: player turn
				//var key = Console.ReadKey(true);
				PlayerAction action = null;
				//switch (key.Key)
				//{
				//case ConsoleKey.C:
				//	action = PlayerAction.CheckOrCall();
				//	break;
				//case ConsoleKey.R:
				//	if (!context.CanRaise)
				//	{
				//		continue;
				//	}
				//	int amount = await RaiseAmount(context.MoneyLeft, context.MinRaise, context.MoneyToCall, context.MyMoneyInTheRound);
				//	action = PlayerAction.Raise(amount);
				//	break;
				//case ConsoleKey.F:
				//	action = PlayerAction.Fold();
				//	break;
				//case ConsoleKey.A:
				//	if (!context.CanRaise)
				//	{
				//		continue;
				//	}

				//	action = context.MoneyLeft > 0
				//				 ? PlayerAction.Raise(context.MoneyLeft - context.MoneyToCall)
				//				 : PlayerAction.CheckOrCall();
				//	break;
				//}

				if (action != null)
				{
					return action;
				}

				await Task.Delay(1);
				passedTime++;

				if (passedTime % 100 == 0)
				{
					Sender.SendAsync(new UpdateTimerSendingData(
										 Guid,
										 context.TimeForTurn - passedTime,
										 Guid,
										 Server.Id,
										 Server.ServerType,
										 (int)lobbyTOclient.UpdateTimer),
									 null);
				}
			}

			return PlayerAction.Fold();
		}

		private int _playerRaiseAmount = -1;

		public void SetPlayerRaiseAmount(int amount)
		{
			//TODORECEIVE:
			_playerRaiseAmount = amount;
		}

		private async Task<int> RaiseAmount(int moneyLeft, int minRaise, int moneyToCall, int myMoneyInTheRound)
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
				var perfix = $"Raise amount [{wholeMinRaise}-{moneyLeft + myMoneyInTheRound}]:";

				do
				{
					ConsoleHelper.WriteOnConsole(Row + 2, 2, new string(' ', Console.WindowWidth - 3));
					ConsoleHelper.WriteOnConsole(Row + 2, 2, perfix);

					//var text = ConsoleHelper.UserInput(Row + 2, perfix.Length + 3);

					while (_playerRaiseAmount == -1)
					{
						await Task.Yield();
					}

					int result = _playerRaiseAmount;

					//if (int.TryParse(text, out result))
					//{
					if (result < wholeMinRaise)
					{
						amount = minRaise;
						break;
					}
					else if (result >= moneyLeft + myMoneyInTheRound)
					{
						// Raise All-in
						amount = moneyLeft - moneyToCall;
						break;
					}

					amount = result - (myMoneyInTheRound + moneyToCall);
					break;
					//}
				}
				while (true);
			}

			_playerRaiseAmount = -1;

			return amount;
		}
	}
}
