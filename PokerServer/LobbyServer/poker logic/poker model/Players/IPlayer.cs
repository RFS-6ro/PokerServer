using System;
using System.Collections;
using System.Threading.Tasks;
using LobbyServer.pokerlogic.Cards;

namespace LobbyServer.pokerlogic.pokermodel.Players
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

		Task<PlayerAction> GetTurn(IGetTurnContext context);

		void EndRound(IEndRoundContext context);

		void EndHand(IEndHandContext context);

		void EndGame(IEndGameContext context);

		Task<PlayerAction> AwaitTurn(IGetTurnContext context);
	}
}
