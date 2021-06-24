using System.Diagnostics;

namespace ServerPool
{
	public interface IServerProcess
	{
		Process Process { get; }

		bool IsResponsable { get; }

		bool CreateProcess();

		void KillProcess();
	}
}
