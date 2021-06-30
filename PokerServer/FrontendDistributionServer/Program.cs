using System;
using System.Threading;
using System.Threading.Tasks;
using ServerDLL;
using UniCastCommonData;

namespace FrontendDistributionServer
{
	class Program
	{
		private static FrontendDistributionServerMediator _mediator;

		static async Task Main(string[] args)
		{
			_mediator = new FrontendDistributionServerMediator(20);

			await _mediator.StartServers(args);

			Thread consoleInput = new Thread(new ThreadStart(() => new ConsoleInput<FrontendDistributionServerMediator>(_mediator)));
			consoleInput.Start();
		}
	}
}

