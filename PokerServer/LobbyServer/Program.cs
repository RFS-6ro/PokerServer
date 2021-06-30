using System;
using System.Threading;
using System.Threading.Tasks;
using ServerDLL;

namespace LobbyServer
{
	class Program
	{
		private static LobbyServerMediator _mediator;

		static async Task Main(string[] args)
		{
			Console.Clear();
			PokerInitializator a = new PokerInitializator();

			_mediator = new LobbyServerMediator(40);

			await _mediator.StartServers(args);

			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine();
			await Task.Factory.StartNew(
				() => { a.Init(); },
				CancellationToken.None,
				TaskCreationOptions.LongRunning,
				SynchronizationContext.Current != null ?
					TaskScheduler.FromCurrentSynchronizationContext() :
					TaskScheduler.Current);
			//Thread consoleInput = new Thread(new ThreadStart(() =>
			new ConsoleInput<LobbyServerMediator>(_mediator);
			//consoleInput.Start();


		}
	}
}
