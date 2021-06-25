using System.Diagnostics;

namespace UniCastCommonData.ServerPool
{
#if false
	public interface IServerProcess
	{
		Process Process { get; }

		bool IsResponsable { get; }

		bool CreateProcess();

		void KillProcess();
	}
#endif
}
