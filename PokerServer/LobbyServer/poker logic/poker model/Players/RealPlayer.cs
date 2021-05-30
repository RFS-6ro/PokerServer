using GameCore.Poker.Controller;
using System;
using System.Collections;
using TexasHoldem.Logic.Players;
using UnityEngine;

namespace GameCore.Poker.Model.Player
{
	public class RealPlayer : BasePlayer
	{
		public override string Name => "Player";

		public override int BuyIn => -1;

		public InputController InputController { get; set; }

		private TexasHoldem.Logic.Players.PlayerAction _currentTurn = null;

		public override TexasHoldem.Logic.Players.PlayerAction GetTurn(IGetTurnContext context)
		{
			//TODO: Set timer value, remove hard coded value
			InputController.GetTurn(context, 5f, (x) => _currentTurn = x);
			return null;
		}

		public override TexasHoldem.Logic.Players.PlayerAction PostingBlind(IPostingBlindContext context)
		{
			return context.BlindAction;
		}

		public override IEnumerator AwaitTurn(Action<TexasHoldem.Logic.Players.PlayerAction> action, IGetTurnContext context)
		{
			GetTurn(context);
			float startTime = Time.time;
			while (_currentTurn == null)
			{
				if (Time.time - startTime >= 5f)
				{
					//_currentTurn = TexasHoldem.Logic.Players.PlayerAction.Fold();
				}
				yield return null;
			}

			action?.Invoke(_currentTurn);
			_currentTurn = null;
		}

		public override void EndHand(IEndHandContext context)
		{
			InputController.Reset();
			base.EndHand(context);
		}
	}
}
