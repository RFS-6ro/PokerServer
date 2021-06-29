using System;
using System.Linq;
using System.IO;
using System.Text;

namespace ServerDLL
{
	public static class StaticLogger
	{
		private static StreamWriter _writer;
		private static StreamReader _reader;

		private static string _name;

		private static object _lock;

		public static void CreateFile(string name)
		{
			lock (_lock)
			{
				_name = name;
				_writer = new StreamWriter(name, true, Encoding.ASCII);
				_reader = new StreamReader(name, Encoding.ASCII);
			}
		}

		public static void Print(object message)
		{
			if (_writer == null)
			{
				return;
			}

			lock (_lock)
			{
				string log = message.ToString();

				Log(log, _writer);
			}
		}

		public static void Print(object[] messages)
		{
			if (_writer == null)
			{
				return;
			}

			lock (_lock)
			{
				string[] logs = messages.Cast<string>().ToArray();

				Log(logs, _writer);
			}
		}

		private static void Log(string logMessage, TextWriter w)
		{
			w.Write("\r\nLog Entry : ");
			w.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
			w.WriteLine("  :");
			w.WriteLine($"  :{logMessage}");
			w.WriteLine("-------------------------------");
		}

		private static void Log(string[] logMessages, TextWriter w)
		{
			w.Write("\r\nLog Entry : ");
			w.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
			w.WriteLine("  :");
			for (int i = 0; i < logMessages.Length; i++)
			{
				w.WriteLine($"  :{logMessages[i]}");
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
