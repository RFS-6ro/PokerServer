using System;
using System.Collections.Generic;
using System.Text;

namespace UniCastCommonData
{
	public class UniCastPacket : IDisposable
	{
		private List<byte> _buffer;
		private byte[] _readableBuffer;

		private int _readPosition;

		/// <summary>Gets the length of the packet's content.</summary>
		public int Length => _buffer.Count; // Return the length of buffer

		/// <summary>Gets the length of the unread data contained in the packet.</summary>
		public int UnreadLength => Length - _readPosition; // Return the remaining length (unread)

		public UniCastPacket(SenderType type, int id) : base()
		{
			_buffer = new List<byte>();
			_readPosition = 0;


		}

		public UniCastPacket(byte[] data)
		{
			_buffer = new List<byte>();
		}

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

		public void Write(byte[] data)
		{

		}

		public byte[] Read()
		{
			return null;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < Length; i++)
			{
				builder.Append(_buffer[i]);
			}
			return builder.ToString();
		}
	}
}
