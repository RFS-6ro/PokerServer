using System;
using PokerSynchronisation;
using UniCastCommonData;
using UniCastCommonData.Observable;

namespace TestingClient
{
	public static class InputModel
	{
		private static bool _isCleared = true;

		public static ObservableVariable<(TurnType, int)> Turn = new ObservableVariable<(TurnType, int)>((TurnType.None, -1));

		public static void ClearTurn()
		{
			Turn.Value = (TurnType.None, -1);
			_isCleared = true;
		}

		public static bool SetTurn(string turnMessage)
		{
			ClearTurn();
			_isCleared = false;
			try
			{
				turnMessage = turnMessage.ToLower();
				if (turnMessage.Contains("r"))
				{
					if (int.TryParse(turnMessage.Split(" ")[1], out int amount))
					{
						Turn.Value = (TurnType.Raise, amount);
					}
					else
					{
						return false;
					}
				}
				else if (turnMessage.Contains("f"))
				{
					Turn.Value = (TurnType.Fold, 0);
				}
				else if (turnMessage.Contains("a"))
				{
					Turn.Value = (TurnType.AllIn, 0);
				}
				else if (turnMessage.Contains("c"))
				{
					Turn.Value = (TurnType.Call, 0);
				}
				else
				{
					return false;
				}

				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}

		public static byte[] GetTurn()
		{
			if (_isCleared)
			{
				return null;
			}

			byte[] data = new byte[8];
			((int)Turn.Value.Item1).ToByteArray().CopyTo(data, 0);
			Turn.Value.Item2.ToByteArray().CopyTo(data, 4);
			ClearTurn();
			return data;
		}
	}
}
