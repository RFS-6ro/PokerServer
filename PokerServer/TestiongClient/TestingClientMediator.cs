using UniCastCommonData.Handlers;

namespace TestingClient
{
	public class TestingClientMediator : AbstractMediator<TestingClientMediator>
	{
		public TestingClientMediator(int ticks = 30) : base(ticks) { }
	}
}
