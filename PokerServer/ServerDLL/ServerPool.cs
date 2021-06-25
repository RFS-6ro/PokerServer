using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCastCommonData.Observable;

namespace ServerPool
{
	public class ServerPool<SERVER_PROCESS>
		where SERVER_PROCESS : class, IServerProcess
	{
		public static ServerPool<SERVER_PROCESS> Instance { get; private set; }

		private ObservableConcurrentDictionary<Guid, SERVER_PROCESS> _servers;
		public IReadOnlyDictionary<Guid, SERVER_PROCESS> Servers => _servers;

		private SERVER_PROCESS _handle;
		public bool IsLocked => _handle != null;

		private Func<SERVER_PROCESS> CreateInstance { get; set; }

		public event ConcurrentDictionaryChangedEventHandler<Guid, SERVER_PROCESS> ItemAdded
		{
			add => _servers.ItemAdded += value;
			remove => _servers.ItemAdded -= value;
		}

		public event ConcurrentDictionaryChangedEventHandler<Guid, SERVER_PROCESS> ItemRemoved
		{
			add => _servers.ItemRemoved += value;
			remove => _servers.ItemRemoved -= value;
		}

		private ServerPool()
		{
			_handle = null;
			_servers = new ObservableConcurrentDictionary<Guid, SERVER_PROCESS>();
		}

		public ServerPool(Func<SERVER_PROCESS> createInstance) : base()
		{
			CreateInstance = createInstance;
		}

		public async Task<bool> CreateNew()
		{
			if (CreateInstance == null)
			{
				return false;
			}

			while (IsLocked)
			{
				await Task.Yield();
			}

			_handle = CreateInstance();

			return _handle == null && (_handle?.CreateProcess() == true);
		}

		public async Task<bool> CreateNew(Func<SERVER_PROCESS> createInstance)
		{
			while (IsLocked)
			{
				await Task.Yield();
			}

			_handle = createInstance();

			return _handle == null && (_handle?.CreateProcess() == true);
		}

		public bool Register(Guid guid)
		{
			if (IsLocked == false)
			{
				return false;
			}

			_servers.Add(guid, _handle);
			_handle = null;

			return true;
		}

		public bool KillProcess(Guid guid)
		{
			bool contains = _servers.TryGetValue(guid, out SERVER_PROCESS value);

			if (contains == false)
			{
				return false;
			}

			value.KillProcess();

			return true;
		}
	}
}
