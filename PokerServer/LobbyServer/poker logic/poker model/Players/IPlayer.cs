using System;
using System.Collections;
using System.Threading.Tasks;
using TexasHoldem.Logic.Cards;

namespace TexasHoldem.Logic.Players
{
	public interface IPlayer
	{
		string Name { get; }

		int BuyIn { get; }

		void StartGame(IStartGameContext context);

		void StartHand(IStartHandContext context);

		void AddCard(Card card, bool isFirstCard);

		void StartRound(IStartRoundContext context);

		PlayerAction PostingBlind(IPostingBlindContext context);

		PlayerAction GetTurn(IGetTurnContext context);

		void EndRound(IEndRoundContext context);

		void EndHand(IEndHandContext context);

		void EndGame(IEndGameContext context);

		Task AwaitTurn(Action<PlayerAction> action, IGetTurnContext context);
	}
}
