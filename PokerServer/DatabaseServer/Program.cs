using System.Threading.Tasks;
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

			//new ConsoleInput<DatabaseServerMediator>(_mediator);
		}
	}
}
