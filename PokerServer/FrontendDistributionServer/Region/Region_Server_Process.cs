using System;
using System.Diagnostics;

namespace FrontendDistributionServer.Region
{
#if false
using UniCastCommonData.ServerPool;
	public class Region_Server_Process : IServerProcess
	{
		private readonly string _filePath;
		private readonly string[] _args;

		public bool IsResponsable => Process != null ? Process.Responding : false;

		public Process Process { get; private set; }

		public Region_Server_Process(string[] args)
		{
			_filePath = args[0];
			_args = args;
		}

		public bool CreateProcess()
		{
			throw new NotImplementedException();
		}

		public void KillProcess()
		{
			Process?.Close();
			Process?.Kill();
			Process?.Dispose();
		}
	}
#endif
}
