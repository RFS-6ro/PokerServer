using System.Diagnostics;

namespace UniCastCommonData.ServerPool
{
	public interface IServerProcess
	{
		Process Process { get; }

		bool IsResponsable { get; }

		bool CreateProcess();

		void KillProcess();
	}
}
