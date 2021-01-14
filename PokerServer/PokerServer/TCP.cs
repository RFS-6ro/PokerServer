using System;
using System.Net.Sockets;

namespace PokerServer
{
	public class TCP
	{
		public TcpClient Socket;

		private readonly int _id;

		private NetworkStream _stream;
		private byte[] _receiveBuffer = new byte[NetworkSettings.DATA_BUFFER_SIZE];

		public TCP(int id)
		{
			_id = id;
		}

		public void Connect(TcpClient socket)
		{
			Socket = socket;
			Socket.ReceiveBufferSize = NetworkSettings.DATA_BUFFER_SIZE;
			Socket.SendBufferSize = NetworkSettings.DATA_BUFFER_SIZE;

			_stream = Socket.GetStream();
			Array.Clear(_receiveBuffer, 0, NetworkSettings.DATA_BUFFER_SIZE);

			_stream.BeginRead(_receiveBuffer, 0, NetworkSettings.DATA_BUFFER_SIZE, ReceiveCallback, null);

			//TODO: send welcome packet
		}

		private void ReceiveCallback(IAsyncResult result)
		{
			try
			{
				int receivedLength = _stream.EndRead(result);

				if (receivedLength <= 0)
				{
					//TODO: disconnect
					ConsoleHandler.PrintWarning("Zero bytes were received. Disconnecting...");

					return;
				}

				byte[] data = new byte[receivedLength];
				Array.Copy(_receiveBuffer, data, receivedLength);
				Array.Clear(_receiveBuffer, 0, receivedLength);

				//TODO: handle received data

				_stream.BeginRead(_receiveBuffer, 0, NetworkSettings.DATA_BUFFER_SIZE, ReceiveCallback, null);
			}
			catch (Exception ex)
			{
				ConsoleHandler.PrintError($"error with receiving data: {ex}");
				//TODO: disconnect
			}
		}
	}
}
