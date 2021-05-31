using UniCastCommonData.Network;

namespace UniCastCommonData.Handlers
{
	public interface IClientInitialisator<CLIENT>
		where CLIENT : TcpClient
	{
		CLIENT Client { get; }

		bool TryStart(string[] args);
	}
}
