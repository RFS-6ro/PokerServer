using System.Diagnostics;

namespace UniCastCommonData.ServerPool
{
	public interface IServerProcess
	{
		Process Process { get; protected set; }

		bool IsResponsable { get; }

		void CreateProcess(string filePath, string[] args);

		void KillProcess(bool force = false);
	}
}
