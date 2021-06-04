using System.Threading.Tasks;
using UniCastCommonData.Handlers;

namespace DatabaseServer
{
	public class DatabaseServerMediator : AbstractMediator<DatabaseServerMediator>
	{
		private Database_Server _server;

		public DatabaseServerMediator(int ticks = 30) : base(ticks) { }

		public async override Task StartServers()
		{
			_server = await StartClientServer();
		}

		public async static Task<Database_Server> StartClientServer()
		{
			return await ServerInitialisator<Database_Server>.StartServer(9090);
		}
	}
}