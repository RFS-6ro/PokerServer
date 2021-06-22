using System;
using System.Net;
using System.Threading.Tasks;
using UniCastCommonData.Network;

namespace UniCastCommonData.Handlers
{
	public static class ServerInitialisator<SERVER>
		where SERVER : TcpServer
	{
		private static SERVER _server;

		public static SERVER Server => _server;

		public async static Task<SERVER> StartServer(int minPort)
		{
			return await StartServer(minPort, minPort);
		}

		public async static Task<SERVER> StartServer(int minPort, int maxPort)
		{
			// TCP server port
			int port = minPort;

			ServerInitResult<SERVER> result = await Task.Run(() =>
			{
				result = TryStart(port);

				while (result.Success == false)
				{
					if (port > maxPort)
					{
						return default;
					}

					Task.Yield();

					result = TryStart(++port);
				}

				return result;
			});

			return result.Server;
		}

		public static ServerInitResult<SERVER> TryStart(int port)
		{
			bool isStarted = false;
			Exception startingException = null;

			try
			{
				Stop();

				_server = (SERVER)Activator.CreateInstance(typeof(SERVER), new object[] { IPAddress.Any, port });
				isStarted = _server.Start();
			}
			catch (Exception ex)
			{
				startingException = ex;
			}

			return new ServerInitResult<SERVER>(isStarted, port, startingException, _server);
		}

		public static bool Restart()
		{
			if (_server != null)
			{
				return false;
			}

			return _server.Restart();
		}

		public static bool Stop()
		{
			if (_server != null)
			{
				return _server.Stop();
			}

			return true;
		}
	}
}
