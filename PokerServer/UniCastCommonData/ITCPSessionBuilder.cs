namespace UniCastCommonData.Network
{
	public interface ITCPSessionBuilder
	{
		TcpSession Create(TcpServer server);
	}
}
