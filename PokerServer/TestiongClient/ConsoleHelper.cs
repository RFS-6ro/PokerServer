using System;
using System.Collections.Generic;
using System.Text;

namespace TestingClient
{
	public static class ConsoleHelper
	{
		public static void WriteOnConsole(int row, int col, string text, ConsoleColor foregroundColor = ConsoleColor.Gray, ConsoleColor backgroundColor = ConsoleColor.Black)
		{
			Console.ForegroundColor = foregroundColor;
			Console.BackgroundColor = backgroundColor;
			Console.SetCursorPosition(col, row);
			Console.Write(text);
		}

		public static string UserInput(int row, int col)
		{
			Console.SetCursorPosition(col, row);
			return Console.ReadLine();
		}

		public static string ToFriendlyString(int type, int suit)
		{
			string friendlyType = " ";
			string friendlySuit = " ";
			switch (suit)
			{
			case -1:
				friendlySuit = " ";
				break;
			case 0:
				friendlySuit = "\u2663"; // ♣
				break;
			case 1:
				friendlySuit = "\u2666"; // ♦
				break;
			case 2:
				friendlySuit = "\u2665"; // ♥
				break;
			case 3:
				friendlySuit = "\u2660"; // ♠
				break;
			case 4:
				friendlySuit = "\u203D"; // unknown
				break;
			default:
				throw new ArgumentException("cardSuit");
			}

			switch (type)
			{
			case -1:
				friendlySuit = " ";
				break;
			case 2:
				friendlyType = "2";
				break;
			case 3:
				friendlyType = "3";
				break;
			case 4:
				friendlyType = "4";
				break;
			case 5:
				friendlyType = "5";
				break;
			case 6:
				friendlyType = "6";
				break;
			case 7:
				friendlyType = "7";
				break;
			case 8:
				friendlyType = "8";
				break;
			case 9:
				friendlyType = "9";
				break;
			case 10:
				friendlyType = "10";
				break;
			case 11:
				friendlyType = "J";
				break;
			case 12:
				friendlyType = "Q";
				break;
			case 13:
				friendlyType = "K";
				break;
			case 14:
				friendlyType = "A";
				break;
			case 15:
				friendlyType = "?";
				break;
			default:
				throw new ArgumentException("cardType");
			}

			return $"{friendlyType}{friendlySuit}";
		}

		public static string CardsToString(this List<(int, int)> cards)
		{
			if (cards == null)
			{
				return string.Empty;
			}

			var stringBuilder = new StringBuilder();
			foreach (var card in cards)
			{
				stringBuilder.Append(ToFriendlyString(card.Item1, card.Item2)).Append(" ");
			}

			return stringBuilder.ToString().Trim();
		}

		public static ConsoleColor GetCardColor(int type, int suit)
		{
			switch (suit)
			{
			case 0: return ConsoleColor.DarkGreen;
			case 1: return ConsoleColor.Red;
			case 2: return ConsoleColor.Red;
			case 3: return ConsoleColor.Black;
			default: return ConsoleColor.DarkRed;
			}
		}
	}
}
