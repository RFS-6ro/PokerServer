using System.Threading;
using System.Threading.Tasks;
using ServerDLL;
using UniCastCommonData;

namespace DatabaseServer
{
	class Program
	{
		private static DatabaseServerMediator _mediator;

		static async Task Main(string[] args)
		{
			_mediator = new DatabaseServerMediator(20);

			await _mediator.StartServers();

			Thread consoleInput = new Thread(new ThreadStart(() => new ConsoleInput<DatabaseServerMediator>(_mediator)));
			consoleInput.Start();
			//new ConsoleInput<DatabaseServerMediator>(_mediator);
		}
	}
}
