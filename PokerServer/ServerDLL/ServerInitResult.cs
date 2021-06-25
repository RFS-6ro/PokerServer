using System;
using UniCastCommonData.Network;

namespace UniCastCommonData.Handlers
{
	public struct ServerInitResult<SERVER>
		where SERVER : TcpServer

	{
		private SERVER _server;
		public SERVER Server => _server;

		private bool _success;
		public bool Success => _success && _exception == null;

		private int _currentPort;
		public int CurrentPort => _currentPort;

		public Exception _exception;
		public Exception Error => _exception;

		public ServerInitResult(bool success, int currentPort, Exception exception, SERVER server)
		{
			_success = success;
			_currentPort = currentPort;
			_exception = exception;
			_server = server;
		}
	}
}
