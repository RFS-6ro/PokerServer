using System;
using System.Threading;
using System.Threading.Tasks;

namespace UniCastCommonData.Handlers
{
	public class AbstractMediator<MEDIATOR> : IStaticInstance<MEDIATOR>
		where MEDIATOR : AbstractMediator<MEDIATOR>
	{
		private static int _ticks = 30;
		CancellationTokenSource _cts;
		public Task _task;

		public static int Ticks => _ticks;

		public event Action OnUpdateEvent;

		public AbstractMediator(int ticks = 30)
		{
			IStaticInstance<MEDIATOR>.Instance = (MEDIATOR)this;
			_ticks = ticks;
		}

		public void Start()
		{
			Console.WriteLine("\tServer Started");

			if (_cts == null)
			{
				_cts = new CancellationTokenSource();

				_task = Task.Factory.StartNew(() => MainThread(_cts.Token), _cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current);
			}
		}

		public virtual async Task StartServers<T>(T param) { }

		public virtual async Task StartServers() { }

		public virtual void OnUpdate() { }

		private void MainThread(CancellationToken token)
		{
			Console.WriteLine("Main thread started. Running at 30 ticks per second.");
			DateTime _nextLoop = DateTime.Now;

			while (token.IsCancellationRequested == false)
			{
				token.ThrowIfCancellationRequested();

				while (_nextLoop < DateTime.Now)
				{
					// If the time for the next loop is in the past, aka it's time to execute another tick
					ThreadManager.UpdateMain(); // Execute game logic
					OnUpdate();
					OnUpdateEvent?.Invoke();

					_nextLoop = _nextLoop.AddMilliseconds(1000f / _ticks); // Calculate at what point in time the next tick should be executed

					if (_nextLoop > DateTime.Now)
					{
						// If the execution time for the next tick is in the future, aka the server is NOT running behind
						Thread.Sleep(_nextLoop - DateTime.Now); // Let the thread sleep until it's needed again.
					}
				}
			}
		}

		public void Stop()
		{
			Console.WriteLine("\tStop called");

			if (_cts != null)
			{
				_cts.Cancel();
				_cts = null;
			}
		}
	}
}
