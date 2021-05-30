using GameCore.SaveSystem;
using System.Collections.Generic;

namespace GameCore.Poker.Contexts
{
    public class GetTurnContext
    {
        public GameRoundType RoundType { get; }
        public IReadOnlyCollection<PlayerActionAndNameBunch> PreviousRoundActions { get; }
        public SafeInt SmallBlind { get; }
        public SafeInt MoneyLeft { get; }
        public SafeInt CurrentPot { get; }
        public SafeInt MyMoneyInTheRound { get; }
        public SafeInt CurrentMaxBet { get; }
        public SafeBool CanCheck => MyMoneyInTheRound == CurrentMaxBet;
        public SafeBool CanRaise => MinRaise > 0 && MoneyLeft > MoneyToCall;
        public SafeInt Bank;
        public SafeInt MoneyToCall
        {
            get
            {
                SafeInt temp = CurrentMaxBet - MyMoneyInTheRound;
                if (temp >= MoneyLeft)
                {
                    // The player does not have enough money to make a full call
                    return MoneyLeft;
                }
                else
                {
                    return temp;
                }
            }
        }
        public SafeBool IsAllIn => MoneyLeft <= 0;
        public SafeInt MinRaise { get; }
        public Pot MainPot { get; }
        public IReadOnlyCollection<Pot> SidePots { get; }

        public GetTurnContext(
            GameRoundType roundType,
            IReadOnlyCollection<PlayerActionAndNameBunch> previousRoundActions,
            SafeInt smallBlind,
            SafeInt moneyLeft,
            SafeInt currentPot,
            SafeInt myMoneyInTheRound,
            SafeInt currentMaxBet,
            SafeInt minRaise,
            Pot mainPot,
            List<Pot> sidePots,
            SafeInt bank)
        {
            RoundType = roundType;
            PreviousRoundActions = previousRoundActions;
            SmallBlind = smallBlind;
            MoneyLeft = moneyLeft;
            CurrentPot = currentPot;
            MyMoneyInTheRound = myMoneyInTheRound;
            CurrentMaxBet = currentMaxBet;
            MinRaise = minRaise;
            MainPot = mainPot;
            SidePots = sidePots;
            Bank = bank;
        }
    }
}
