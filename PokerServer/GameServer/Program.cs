﻿using System;
using System.Threading;
using Network;

namespace GameServer
{
	class Program
	{
		private static bool isRunning = false;

		static void Main(string[] args)
		{
			Console.Title = "Poker Server";
			isRunning = true;

			Thread mainThread = new Thread(new ThreadStart(MainThread));
			mainThread.Start();
			MainGameServer.Instance.Start(10, NetworkSettings.TEST_SERVERPORT);

			Console.ReadLine();
		}

		private static void MainThread()
		{
			Console.WriteLine($"Main thread started. Running at {Constants.TICKS_PER_SEC} ticks per second.");
			DateTime _nextLoop = DateTime.Now;

			while (isRunning)
			{
				while (_nextLoop < DateTime.Now)
				{
					// If the time for the next loop is in the past, aka it's time to execute another tick
					ThreadManager.UpdateMain(); // Execute game logic


					_nextLoop = _nextLoop.AddMilliseconds(Constants.MS_PER_TICK); // Calculate at what point in time the next tick should be executed

					if (_nextLoop > DateTime.Now)
					{
						// If the execution time for the next tick is in the future, aka the server is NOT running behind
						Thread.Sleep(_nextLoop - DateTime.Now); // Let the thread sleep until it's needed again.
					}
				}
			}
		}
	}
}
