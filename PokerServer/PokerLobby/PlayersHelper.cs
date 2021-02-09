using System.Collections.Generic;
using System.Linq;

namespace PokerLobby
{
	public static class PlayersHelper
	{
		public static IEnumerable<RealPlayerDecorator> WithMoney(this IEnumerable<RealPlayerDecorator> players) =>
			players.Where(p => p.PlayerMoney.Money > 0);

		public static IEnumerable<RealPlayerDecorator> InThisHand(this IEnumerable<RealPlayerDecorator> players) =>
			players.Where(p => p.PlayerMoney.InHand);
	}
}
