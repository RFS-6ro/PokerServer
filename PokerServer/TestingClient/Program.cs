using System;
using System.Threading;
using System.Threading.Tasks;

namespace TestingClient
{
	class Program
	{
		private static TestingClientMediator _mediator;

		static void Main(string[] args)
		{
			_mediator = new TestingClientMediator(60);
			new PokerInitializer();

			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine();

			//await
			_mediator.StartServers();

			//Thread consoleInput = new Thread(new ThreadStart(() =>
			new PokerPlayerConsoleInput(_mediator);
			//consoleInput.Start();
		}
	}
}
