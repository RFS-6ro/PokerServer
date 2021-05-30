using System.Collections.Generic;

namespace GameCore.Poker.Contexts
{
    public class EndRoundContext
    {
        public GameRoundType RoundType;

        public IReadOnlyCollection<PlayerActionAndNameBunch> RoundActions { get; }

        public EndRoundContext(IReadOnlyCollection<PlayerActionAndNameBunch> roundActions, GameRoundType roundType)
        {
            RoundActions = roundActions;
            RoundType = roundType;
        }
    }
}
