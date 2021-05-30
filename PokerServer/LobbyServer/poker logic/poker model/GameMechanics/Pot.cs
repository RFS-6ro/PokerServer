namespace TexasHoldem.Logic.GameMechanics
{
	using System.Collections.Generic;

	public struct Pot
	{
		public Pot(int amountOfMoney, IReadOnlyList<string> activePlayer)
		{
			AmountOfMoney = amountOfMoney;
			ActivePlayer = activePlayer;
		}

		public int AmountOfMoney { get; }

		public IReadOnlyList<string> ActivePlayer { get; }
	}
}
