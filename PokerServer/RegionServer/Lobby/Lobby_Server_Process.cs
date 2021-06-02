using System;
using System.Diagnostics;
using UniCastCommonData.ServerPool;

namespace RegionServer.Lobby
{
	public class Lobby_Server_Process : IServerProcess
	{
		private readonly string _filePath;
		private readonly string[] _args;

		public bool IsResponsable => Process != null ? Process.Responding : false;

		public Process Process { get; private set; }

		public Lobby_Server_Process(string[] args)
		{
			_filePath = args[0];
			_args = args;
		}

		public bool CreateProcess()
		{
			return false;
		}

		public void KillProcess()
		{
			Process?.Close();
			Process?.Kill();
			Process?.Dispose();
		}
	}
}
