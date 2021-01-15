using System;
using System.Collections.Generic;
using System.Text;

namespace PokerServer
{
	/// <summary>Sent from server to client.</summary>
	public enum ServerPackets
	{
		welcome = 1,
		udpTest
	}

	/// <summary>Sent from client to server.</summary>
	public enum ClientPackets
	{
		welcomeReceived = 1,
		updTestReceived
	}

	public partial class Packet : IDisposable
	{
		private List<byte> _buffer;
		private byte[] _readableBuffer;
		private int _readPosition;

		/// <summary>Gets the length of the packet's content.</summary>
		public int Length => _buffer.Count; // Return the length of buffer

		/// <summary>Gets the length of the unread data contained in the packet.</summary>
		public int UnreadLength => Length - _readPosition; // Return the remaining length (unread)

		/// <summary>Creates a new empty packet (without an ID).</summary>
		public Packet()
		{
			_buffer = new List<byte>(); // Intitialize buffer
			_readPosition = 0; // Set readPos to 0
		}

		/// <summary>Creates a new packet with a given ID. Used for sending.</summary>
		/// <param name="id">The packet ID.</param>
		public Packet(int id)
		{
			_buffer = new List<byte>(); // Intitialize buffer
			_readPosition = 0; // Set readPos to 0

			Write(id); // Write packet id to the buffer
		}

		/// <summary>Creates a packet from which data can be read. Used for receiving.</summary>
		/// <param name="data">The bytes to add to the packet.</param>
		public Packet(byte[] data)
		{
			_buffer = new List<byte>(); // Intitialize buffer
			_readPosition = 0; // Set readPos to 0

			SetBytes(data);
		}

		#region Functions
		/// <summary>Sets the packet's content and prepares it to be read.</summary>
		/// <param name="data">The bytes to add to the packet.</param>
		public void SetBytes(byte[] data)
		{
			Write(data);
			_readableBuffer = _buffer.ToArray();
		}

		/// <summary>Inserts the length of the packet's content at the start of the buffer.</summary>
		public void WriteLength()
		{
			_buffer.InsertRange(0, BitConverter.GetBytes(_buffer.Count)); // Insert the byte length of the packet at the very beginning
		}

		/// <summary>Inserts the given int at the start of the buffer.</summary>
		/// <param name="value">The int to insert.</param>
		public void InsertInt(int value)
		{
			_buffer.InsertRange(0, BitConverter.GetBytes(value)); // Insert the int at the start of the buffer
		}

		/// <summary>Gets the packet's content in array form.</summary>
		public byte[] ToArray()
		{
			_readableBuffer = _buffer.ToArray();
			return _readableBuffer;
		}

		/// <summary>Resets the packet instance to allow it to be reused.</summary>
		/// <param name="shouldReset">Whether or not to reset the packet.</param>
		public void Reset(bool shouldReset = true)
		{
			if (shouldReset)
			{
				_buffer.Clear(); // Clear buffer
				_readableBuffer = null;
				_readPosition = 0; // Reset readPos
			}
			else
			{
				_readPosition -= 4; // "Unread" the last read int
			}
		}
		#endregion

		private bool disposed = false;

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					_buffer = null;
					_readableBuffer = null;
					_readPosition = 0;
				}

				disposed = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}