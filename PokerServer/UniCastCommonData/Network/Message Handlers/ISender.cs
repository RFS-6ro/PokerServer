namespace UniCastCommonData.Network.MessageHandlers
{
	public interface ISender
	{
		bool SendAsync(UniCastPacket packet);
	}
}
