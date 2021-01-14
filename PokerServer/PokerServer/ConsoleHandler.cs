using System;
using System.Threading.Tasks;

namespace PokerServer
{
	public static class ConsoleHandler
	{
		public static void PrintSuccess(string message)
		{
			PrintColored(message, ConsoleColor.Green, ConsoleColor.Black);
		}

		public static void PrintWarning(string message)
		{
			PrintColored(message, ConsoleColor.Yellow, ConsoleColor.Black);
		}

		public static void PrintError(string message)
		{
			PrintColored(message, ConsoleColor.Red, ConsoleColor.White);
		}

		public static void PrintColored(string message, ConsoleColor backgroundColor = ConsoleColor.Black, ConsoleColor textColor = ConsoleColor.White)
		{
			Console.BackgroundColor = backgroundColor;
			Console.ForegroundColor = textColor;
			Console.WriteLine(message);
			Console.ResetColor();
		}
	}
}
