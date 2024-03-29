﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace UniCastCommonData.Handlers
{
	public class AbstractMediator<MEDIATOR> where MEDIATOR : AbstractMediator<MEDIATOR>
	{
		internal static int _ticks = 30;
		internal CancellationTokenSource _cts;
		public Task MainThreadTask;

		public static int Ticks => _ticks;

		public event Action OnUpdateEvent;

		public static MEDIATOR Instance { get; protected set; }

		public AbstractMediator(int ticks = 30)
		{
			Instance = (MEDIATOR)this;
			_ticks = ticks;
		}

		public void Start()
		{
			Console.WriteLine("\tServer Started");
			if (_cts == null)
			{
				_cts = new CancellationTokenSource();

				MainThreadTask = Task.Factory.StartNew(
					() => { try { MainThread(() => _cts.Token.IsCancellationRequested); } catch { } },
					_cts.Token,
					TaskCreationOptions.LongRunning, TaskScheduler.Current);
			}
		}

		public virtual void OnUpdate() { }

		protected virtual void MainThread(Func<bool> cancelationCondition)
		{
			Console.WriteLine("Main thread started. Running at 30 ticks per second.");
			DateTime _nextLoop = DateTime.Now;

			while (cancelationCondition() == false)
			{
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
