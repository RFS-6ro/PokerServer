using System;
using System.Net;
using System.Net.Sockets;
using Network;

namespace FrontendServer
{
	public class FrontendServerDistributor : SingletonBase<FrontendServerDistributor>, IServer, ITCPConnection, IUDPConnection
	{

		public void Start(int _maxPlayers, int _port)
		{
			IServer.MaxPlayers = _maxPlayers;
			IServer.Port = _port;

			Console.WriteLine("Starting server...");
			InitializeServerData();

			ITCPConnection._tcpListener = new TcpListener(IPAddress.Any, Port);
			ITCPConnection._tcpListener.Start();
			ITCPConnection._tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);

			_udpListener = new UdpClient(IServer.Port);
			_udpListener.BeginReceive(UDPReceiveCallback, null);

			Console.WriteLine($"Server started on port {IServer.Port}.");
		}

		public void InitializeServerData()
		{
		}

		void ITCPConnection.TCPConnectCallback(IAsyncResult _result)
		{
			throw new NotImplementedException();
		}

		void IUDPConnection.UDPReceiveCallback(IAsyncResult _result)
		{
			throw new NotImplementedException();
		}

		public void SendUDPData(IPEndPoint _clientEndPoint, Packet _packet)
		{
			throw new NotImplementedException();
		}
	}
}
