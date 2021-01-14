using System;
using System.Net;
using System.Net.Sockets;

namespace PokerServer
{
	public class Client
	{
		public int Id;
		public TCP Tcp;

		public Client(int clientId)
		{
			Id = clientId;
			Tcp = new TCP(Id);
		}
	}
}
