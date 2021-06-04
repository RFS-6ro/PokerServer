using System;
using System.Threading.Tasks;
using TexasHoldem.Logic.Players;

namespace GameCore.Poker.Model.Player
{
	public class RealPlayer : BasePlayer
	{
		public override string Name => "Player";

		public override int BuyIn => -1;

		//public InputController InputController { get; set; }

		private PlayerAction _currentTurn = null;

		public override PlayerAction GetTurn(IGetTurnContext context)
		{
			//TODO: Set timer value, remove hard coded value
			//InputController.GetTurn(context, 5f, (x) => _currentTurn = x);
			return null;
		}

		public override PlayerAction PostingBlind(IPostingBlindContext context)
		{
			return context.BlindAction;
		}

		public override async Task AwaitTurn(Action<PlayerAction> action, IGetTurnContext context)
		{
			GetTurn(context);
			//float startTime = Time.time;
			while (_currentTurn == null)
			{
				//if (Time.time - startTime >= 5f)
				{
					//_currentTurn = PlayerAction.Fold();
				}
				await Task.Delay(1);
			}

			action?.Invoke(_currentTurn);
			_currentTurn = null;
		}

		public override void EndHand(IEndHandContext context)
		{
			//InputController.Reset();
			base.EndHand(context);
		}
	}
}
