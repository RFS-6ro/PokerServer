using UniCastCommonData.Handlers;

namespace FrontendDistributionServer
{
	public class FrontendDistributionServerMediator : AbstractMediator<FrontendDistributionServerMediator>
	{
		public FrontendDistributionServerMediator(int ticks = 30) : base(ticks) { }
	}
}
