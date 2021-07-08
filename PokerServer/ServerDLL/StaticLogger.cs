using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace ServerDLL
{
	public static class StaticLogger
	{
		private struct Log
		{
			public DateTime Time;
			public string Identificator;
			public IEnumerable<string> Messages;

			public Log(DateTime time, string identificator, IEnumerable<string> messages)
			{
				Time = time;
				Identificator = identificator;
				Messages = messages;
			}
		}

		private static List<Log> Logs = new List<Log>();

		private static StreamWriter _writer;
		private static StreamReader _reader;

		private static string _name;

		private static object _lock = new object();


		public static void CreateFile(string name)
		{
			lock (_lock)
			{
				_name = name;
				_writer = new StreamWriter(name, true, Encoding.ASCII);
				_reader = new StreamReader(name, Encoding.ASCII);
			}
		}


		public static void Filter(string[] filters)
		{
			try
			{
				lock (_lock)
				{
					if (filters.Length == 2)
					{
						throw null;
					}

					IEnumerable<Log> filtered = Logs.Where((x) => x.Identificator.Contains(filters[2]));
					var filteredWriter = new StreamWriter(filters[3], true, Encoding.ASCII);
					foreach (var log in filtered)
					{
						WriteLogToFile(log, filteredWriter);
					}
					filteredWriter.Close();
				}
			}
			catch
			{
				Console.WriteLine(@"Wrong fiter signature. It should be: log filter %Identificator% %FileName.*%");
			}
		}

		public static void Print(object message)
		{
			Print("default", message.ToString());
		}

		public static void Print(string log)
		{
			Print("default", log);
		}

		public static void Print(string identificator, object message)
		{
			Print(identificator, message.ToString());
		}

		public static void Print(string identificator, string log)
		{
			Print(identificator, new string[] { log });
		}

		public static void Print(object[] messages)
		{
			string[] logs = messages.Cast<string>().ToArray();
			Print("default", logs);
		}

		public static void Print(string[] logs)
		{
			Print("default", logs);
		}

		public static void Print(string identificator, object[] messages)
		{
			string[] logs = messages.Cast<string>().ToArray();

			Print(identificator, logs);
		}

		public static void Print(string identificator, string[] logs)
		{
			Print(identificator, (IEnumerable<string>)logs);
		}

		public static void Print(string identificator, IEnumerable<string> logs)
		{
			lock (_lock)
			{
				Log currentLog = new Log(DateTime.Now, identificator, logs);
				Logs.Add(currentLog);

				if (_writer == null)
				{
					return;
				}

				WriteLogToFile(currentLog, _writer);
			}
		}

		private static void WriteLogToFile(Log log, TextWriter w)
		{
			w.Write($"\r\nLog Entry {log.Identificator}: ");
			w.WriteLine($"{log.Time.ToLongTimeString()} {log.Time.ToLongDateString()}");
			w.WriteLine("  :");
			foreach (var message in log.Messages)
			{
				w.WriteLine($"  :{message}");
			}
			w.WriteLine("-------------------------------");
		}

		public static void WriteLogToConsole()
		{
			lock (_lock)
			{
				if (_reader == null)
				{
					Console.WriteLine("file is missing");
					return;
				}

				DumpLog(_reader);
			}
		}

		private static void DumpLog(StreamReader r)
		{
			string line;
			while ((line = r.ReadLine()) != null)
			{
				Console.WriteLine(line);
			}
		}


		public static void CloseFile()
		{
			lock (_lock)
			{
				_writer?.Close();
				_reader?.Close();
			}
		}
	}
}
