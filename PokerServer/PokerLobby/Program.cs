using System;

namespace PokerLobby
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				Console.WriteLine(args[0]);
			}
			catch (Exception ex)
			{
				Console.WriteLine("exception");
			}

			Console.ReadLine();
			Console.ReadLine();
		}
	}
}
