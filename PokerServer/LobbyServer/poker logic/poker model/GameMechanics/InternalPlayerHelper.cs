namespace TexasHoldem.Logic.GameMechanics
{
    using System.Collections.Generic;
    using System.Linq;

    using TexasHoldem.Logic.Players;

    public static class InternalPlayerHelper
    {
        public static IEnumerable<T> WithMoney<T>(this IEnumerable<T> players)
            where T : PlayerDecorator => players.Where(p => p.PlayerMoney.Money > 0);

        public static IEnumerable<T> InThisHand<T>(this IEnumerable<T> players)
            where T : PlayerDecorator => players.Where(p => p.PlayerMoney.InHand);
    }
}
