using System;
using System.Collections.Generic;

namespace UniCastCommonData.ServerPool
{
	public class ServerPool<SERVER_PROCESS> : IStaticInstance<ServerPool<SERVER_PROCESS>>
		where SERVER_PROCESS : IServerProcess
	{
		private Stack<SERVER_PROCESS> _avaliableServers { get; set; }


		public int Count => _avaliableServers.Count;


		private Func<SERVER_PROCESS> CreateInstance { get; set; }


		private ServerPool()
		{
			_avaliableServers = new Stack<SERVER_PROCESS>();
		}

		public ServerPool(Func<SERVER_PROCESS> createInstance) : base()
		{
			CreateInstance = createInstance;
		}

		public ServerPool(Func<SERVER_PROCESS> createInstance, int maxCapacity)
		{
			_avaliableServers = new Stack<SERVER_PROCESS>(maxCapacity);
			CreateInstance = createInstance;
		}

		public SERVER_PROCESS GetServer()
		{
			if (_avaliableServers.Count <= 0)
			{
				if (CreateInstance != null)
				{
					return CreateInstance();
				}
			}

			return _avaliableServers.Pop();
		}

		public void ReturnServer(SERVER_PROCESS server)
		{
			_avaliableServers.Push(server);
		}
	}
}
