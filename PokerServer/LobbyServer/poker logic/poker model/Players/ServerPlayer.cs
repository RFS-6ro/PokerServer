using System;
using TexasHoldem.Logic.Players;

namespace LobbyServer.pokerlogic.pokermodel.Players
{
	public class ServerPlayer : BasePlayer
	{
		public override string Name => throw new NotImplementedException();

		public override int BuyIn => throw new NotImplementedException();

		public override PlayerAction GetTurn(IGetTurnContext context)
		{
			throw new NotImplementedException();
		}

		public override PlayerAction PostingBlind(IPostingBlindContext context)
		{
			throw new NotImplementedException();
		}
	}
}
