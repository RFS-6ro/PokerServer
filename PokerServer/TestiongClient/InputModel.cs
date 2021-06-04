using PokerSynchronisation;
using UniCastCommonData;

namespace TestingClient
{
	public static class InputModel
	{
		private static TurnType _turnType;
		private static int _amount;

		public static void SetTurn(string turnMessage)
		{
			turnMessage = turnMessage.ToLower();
			if (turnMessage.Contains("r"))
			{
				_turnType = TurnType.Raise;

				if (int.TryParse(turnMessage.Split(" ")[1], out int amount))
				{
					_amount = amount;
				}
			}
			else
			if (turnMessage.Contains("f"))
			{
				_turnType = TurnType.Fold;
			}
			else
			if (turnMessage.Contains("a"))
			{
				_turnType = TurnType.AllIn;
			}
			if (turnMessage.Contains("c"))
			{
				_turnType = TurnType.Call;
			}
		}

		public static byte[] GetTurn()
		{
			if (_turnType == TurnType.None)
			{
				return null;
			}
			byte[] data = new byte[8];
			((int)_turnType).ToByteArray().CopyTo(data, 0);
			(_amount).ToByteArray().CopyTo(data, 4);
			_turnType = TurnType.None;
			return data;
		}
	}
}
