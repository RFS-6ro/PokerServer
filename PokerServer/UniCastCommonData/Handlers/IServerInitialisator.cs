using System;
using UniCastCommonData.Network;

namespace UniCastCommonData.Handlers
{
	public interface IServerInitialisator<SERVER>
		where SERVER : TcpServer
	{
		SERVER Server { get; }

		bool TryStart(string[] args);
	}
}
