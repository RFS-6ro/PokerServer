using System.Collections.Generic;

namespace GameCore.Poker.Contexts
{
    public class StartGameContext
    {
        public IReadOnlyCollection<string> PlayerNames { get; }

        public StartGameContext(IReadOnlyCollection<string> playerNames)
        {
            PlayerNames = playerNames;
        }
    }
}
