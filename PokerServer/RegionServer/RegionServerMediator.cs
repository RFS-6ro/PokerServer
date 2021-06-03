using UniCastCommonData.Handlers;

namespace RegionServer
{
	public class RegionServerMediator : AbstractMediator<RegionServerMediator>
	{
		public RegionServerMediator(int ticks = 30) : base(ticks) { }
	}
}
