using System;

namespace PokerServer
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.Title = "Poker Server";

			Server.Start(50, NetworkSettings.PORT);


			Console.ReadLine();
		}
	}
}
