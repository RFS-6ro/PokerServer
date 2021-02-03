using System;
using System.Threading.Tasks;
using TexasHoldem.Logic.Players;

namespace PokerLobby
{
	public class RealPlayer : BasePlayer
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
