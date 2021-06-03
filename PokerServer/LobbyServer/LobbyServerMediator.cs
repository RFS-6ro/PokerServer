using UniCastCommonData.Handlers;

namespace LobbyServer
{
	public class LobbyServerMediator : AbstractMediator<LobbyServerMediator>
	{
		public LobbyServerMediator(int ticks = 30) : base(ticks) { }
	}
}
