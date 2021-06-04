using System.Threading.Tasks;
using UniCastCommonData;

namespace RegionServer
{
	class Program
	{
		private static RegionServerMediator _mediator;

		static async Task Main(string[] args)
		{
			_mediator = new RegionServerMediator();

			await _mediator.StartServers(args);

			new ConsoleInput<RegionServerMediator>(_mediator);
		}
	}
}
