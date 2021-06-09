using System.Threading.Tasks;
using UniCastCommonData;

namespace LobbyServer
{
	class Program
	{
		private static LobbyServerMediator _mediator;

		static async Task Main(string[] args)
		{
			PokerInitializator a = new PokerInitializator();

			_mediator = new LobbyServerMediator(40);

			await _mediator.StartServers(args);

			new ConsoleInput<LobbyServerMediator>(_mediator);
		}
	}
}
