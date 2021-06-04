using System;
using System.Collections;
using System.Threading.Tasks;
using LobbyServer.pokerlogic.controllers;
using TexasHoldem.Logic.Players;

namespace LobbyServer.pokerlogic.pokermodel.Players
{
	public class UnityPlayerDecorator : PlayerDecorator
	{
		protected ChairViewModel _chairView;

		public ChairViewModel ChairView => _chairView;

		protected bool _isDealer;
		public bool IsDealer => _isDealer;

		public UnityPlayerDecorator() { }

		//TODO: ADD LINKS TO BASIC VIEWS
		public UnityPlayerDecorator(ChairViewModel chairView)
		{
			_chairView = chairView;
		}

		public override void SetPlayer(IPlayer player)
		{
			base.SetPlayer(player);
		}

		public void ConfigureSeat()
		{
		}

		public override void StartGame(IStartGameContext context)
		{
			base.StartGame(context);
			_chairView?.SetMoneyView(PlayerMoney.Money);
		}

		public override void StartHand(IStartHandContext context)
		{
			_isDealer = context.FirstPlayerName == Player.Name;

			_chairView.SetGameStateHolder(string.Empty);

			base.StartHand(context);
			_chairView?.SetMoneyView(PlayerMoney.Money);
		}

		public override PlayerAction PostingBlind(IPostingBlindContext context)
		{
			string state;
			state = context.CurrentPot == 0 ? "SB" : "BB";
			_chairView.SetGameStateHolder(state);

			PlayerAction blindAction = base.PostingBlind(context);
			_chairView?.SetMoneyView(PlayerMoney.Money);

			return blindAction;
		}

		public override void StartRound(IStartRoundContext context)
		{
			base.StartRound(context);
			_chairView?.SetMoneyView(PlayerMoney.Money);
		}

		public override PlayerAction GetTurn(IGetTurnContext context)
		{
			//_chairView.StartTimer(0f);//TODO: time check

			PlayerAction turn = base.GetTurn(context);
			_chairView?.SetMoneyView(PlayerMoney.Money);

			//_chairView.StopTimer();

			return turn;
		}

		public override void EndRound(IEndRoundContext context)
		{
			base.EndRound(context);
			_chairView?.SetMoneyView(PlayerMoney.Money);

			//TODO: send guid
			//BetController.RemoveBet(_chairView);
		}

		public override void EndHand(IEndHandContext context)
		{
			base.EndHand(context);
			_chairView?.SetMoneyView(PlayerMoney.Money);
		}

		public override void EndGame(IEndGameContext context)
		{
			base.EndGame(context);
		}

		public override async Task AwaitTurn(Action<PlayerAction> action, IGetTurnContext context)
		{
			_chairView?.StartTurn();

			if (_chairView != null)
			{
				_chairView.StartTimer(context.TimeForTurn);
				//TODO: sendTimer
				//action += (x) => _chairView.StartTimer();
			}
			await base.AwaitTurn(action, context);
			_chairView?.SetMoneyView(PlayerMoney.Money);

			_chairView?.EndTurn();
		}
	}
}
