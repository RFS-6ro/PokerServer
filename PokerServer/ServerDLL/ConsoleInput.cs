using System;
using UniCastCommonData.Handlers;

namespace ServerDLL
{
	public class ConsoleInput<MEDIATOR>
		where MEDIATOR : AbstractMediator<MEDIATOR>
	{
		protected bool _isRunning;
		protected MEDIATOR _mediator;

		public ConsoleInput(MEDIATOR mediator)
		{
			_mediator = mediator;
			_mediator.Start();

			_isRunning = true;

			while (_isRunning)
			{
				// check for user input
				checkInput(Console.ReadLine());
			}

			// wait for the task to finish before exiting
			_mediator.MainThreadTask.Wait();
		}

		protected virtual void checkInput(string input)
		{
			if (input == "quit")
			{
				Console.WriteLine("\nQuit called from main thread");

				_mediator.Stop();
				_isRunning = false;
				return;
			}

			if (input.Contains("log start"))
			{
				string filename = input.Split(' ')[2];
				StaticLogger.CreateFile(filename);
				return;
			}

			if (input == "log stop")
			{
				StaticLogger.CloseFile();
				return;
			}

			if (input == "log print")
			{
				StaticLogger.WriteLogToConsole();
				return;
			}

			if (input.Contains("log filter"))
			{
				string[] filters = input.Split(' ');
				StaticLogger.Filter(filters);
				return;
			}
		}
	}
}
