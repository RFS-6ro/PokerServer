using GameCore.SaveSystem;

namespace GameCore.Poker.Contexts
{
    public class PostingBlindContext
    {
        public PlayerAction BlindAction { get; }
        public SafeInt CurrentPot { get; }
        public SafeInt CurrentStackSize { get; }

        public PostingBlindContext(PlayerAction blindAction, SafeInt currentPot, SafeInt currentStackSize)
        {
            BlindAction = blindAction;
            CurrentPot = currentPot;
            CurrentStackSize = currentStackSize;
        }
    }
}
