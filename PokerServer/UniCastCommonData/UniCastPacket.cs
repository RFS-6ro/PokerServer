using System;
using System.Collections.Generic;
using System.Text;
using UniCastCommonData.Handlers;
using UniCastCommonData.Wrappers;

namespace UniCastCommonData
{
	public class UniCastPacket : IDisposable, IByteArrayConvertable
	{
		private List<byte> _buffer;
		private byte[] _readableBuffer;
		private int _readPosition;
		private bool disposed = false;

		public int Length => _buffer.Count;
		public int UnreadLength => Length - _readPosition;

		public UniCastPacket(ActorType sender) : base()
		{
			_buffer = new List<byte>();
			_readPosition = 0;
			Write(BitConverter.GetBytes((int)sender));
		}

		public UniCastPacket(ActorType sender, int id) : base()
		{
			_buffer = new List<byte>();
			_readPosition = 0;
			Write(BitConverter.GetBytes((int)sender));
			Write(BitConverter.GetBytes(id));
		}

		public UniCastPacket(byte[] data)
		{
			_buffer = new List<byte>();
			_readPosition = 0;
			SetBytes(data);
		}

		private void SetBytes(byte[] data)
		{
			Write(data);
			_readableBuffer = _buffer.ToArray();
		}

		public void WriteLength()
		{
			_buffer.InsertRange(0, BitConverter.GetBytes(Length));
		}

		public void InsertInt(int value)
		{
			_buffer.InsertRange(0, BitConverter.GetBytes(value));
		}

		public void Write(int value)
		{
			_buffer.AddRange(value.ToByteArray());
		}

		public void Write<T>(T data) where T : IByteArrayConvertable
		{
			Write(data.GetRawBytes());
		}

		public void Write(byte[] data)
		{
			_buffer.AddRange(data);
		}

		public int ReadInt(bool moveReadPos = true)
		{
			return Read(4, moveReadPos).ToInt32();
		}

		public T Read<T>(bool moveReadPos = true) where T : IBasePacketDataWrapper, new()
		{
			T data = new T();
			data.SetTypedData(Read(data.RawDataLength, moveReadPos));
			return data;
		}

		public byte[] Read(int length, bool moveReadPos = true)
		{
			if (_buffer.Count > _readPosition)
			{
				byte[] value = _buffer.GetRange(_readPosition, length).ToArray();
				if (moveReadPos)
				{
					_readPosition += length;
				}
				return value;
			}
			else
			{
				throw new Exception("Could not read value of type 'byte[]'!");
			}
		}

		public byte[] GetRawBytes()
		{
			_readableBuffer = _buffer.ToArray();
			return _readableBuffer;
		}

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
