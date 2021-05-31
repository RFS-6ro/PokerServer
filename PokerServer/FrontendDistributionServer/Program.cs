using System;
using System.Net;
using FrontendDistributionServer.ClientSide;

namespace FrontendDistributionServer
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.Title = "Frontend Distribution Server";
			// TCP server address
			string address = "127.0.0.1";
			if (args.Length > 0)
				address = args[0];

			// TCP server port
			int port = 1111;
			if (args.Length > 1)
				port = int.Parse(args[1]);

			Console.WriteLine($"Frontend Distribution TCP server address: {address}");
			Console.WriteLine($"Frontend Distribution TCP server port: {port}");

			Console.WriteLine();

			var server = new FrontendDistribution_Client_Server(IPAddress.Any, port);
			// server.OptionNoDelay = true;
			server.OptionReuseAddress = true;
			// Start the server
			Console.Write("Server starting...");
			server.Start();

			while (true)
			{
				Console.ReadLine();
			}

			//send example
			//server.SendHandler.Handlers[Handlers.frontendTOclient.Count]?.Invoke(null);
		}
	}
}
