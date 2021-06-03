using System;
using System.Threading;
using System.Threading.Tasks;
using UniCastCommonData.Handlers;

namespace UniCastCommonData
{
	public class ConsoleInput<MEDIATOR>
		where MEDIATOR : AbstractMediator<MEDIATOR>
	{
		static bool _running;
		static MEDIATOR _mediator;

		public ConsoleInput(MEDIATOR mediator)
		{
			_mediator = mediator;
			_mediator.Start();

			_running = true;

			while (_running)
			{
				// check for user input
				checkInput(Console.ReadKey().KeyChar);
			}

			// wait for the task to finish before exiting
			_mediator._task.Wait();
		}

		static void checkInput(char input)
		{
			if (input == 'q')
			{
				Console.WriteLine("\nQuit called from main thread");

				_mediator.Stop();
				_running = false;
			}
		}
	}
}
