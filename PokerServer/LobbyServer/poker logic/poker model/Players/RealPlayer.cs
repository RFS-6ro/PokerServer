using System;
using System.Threading.Tasks;

namespace LobbyServer.pokerlogic.pokermodel.Players
{
	public class RealPlayer : BasePlayer
	{
		public override string Name => "Player";

		public override int BuyIn => -1;

		//public InputController InputController { get; set; }

		private PlayerAction _currentTurn = null;

		public override Task<PlayerAction> GetTurn(IGetTurnContext context)
		{
			//TODO: Set timer value, remove hard coded value
			//InputController.GetTurn(context, 5f, (x) => _currentTurn = x);
			return null;
		}

		public override PlayerAction PostingBlind(IPostingBlindContext context)
		{
			return context.BlindAction;
		}

		public override async Task<PlayerAction> AwaitTurn(IGetTurnContext context)
		{
			var turn = await GetTurn(context);
			//float startTime = Time.time;
			return turn;
		}

		public override void EndHand(IEndHandContext context)
		{
			//InputController.Reset();
			base.EndHand(context);
		}
	}
}
