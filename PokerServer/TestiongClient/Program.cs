using System.Threading.Tasks;
using UniCastCommonData;

namespace TestingClient
{
	class Program
	{
		private static TestingClientMediator _mediator;

		static async Task Main(string[] args)
		{
			_mediator = new TestingClientMediator(60);

			_mediator.StartServers();

			new PokerPlayerConsoleInput(_mediator);
		}
	}
}
