using System;

namespace RegionServer
{
	class Program
	{
		static void Main(string[] args)
		{
			// TCP server address
			string address = "127.0.0.1";
			if (args.Length > 0)
				address = args[0];

			// TCP server port
			int port = 6379;
			if (args.Length > 1)
				port = int.Parse(args[1]);

			Console.WriteLine($"Region TCP server address: {address}");
			Console.WriteLine($"Region TCP server port: {port}");

			Console.WriteLine();
		}
	}
}
