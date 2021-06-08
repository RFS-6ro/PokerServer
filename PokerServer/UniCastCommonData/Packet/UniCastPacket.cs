using System;
using System.Collections.Generic;
using System.Text;
using UniCastCommonData.Handlers;
using UniCastCommonData.Wrappers;
using UniCastCommonData.Packet.InitialDatas;

namespace UniCastCommonData
{
	public class UniCastPacket : IDisposable, IByteArrayConvertable
	{
		private List<byte> _buffer = new List<byte>();
		private byte[] _readableBuffer;
		private int _readPosition = 0;
		private bool disposed = false;

		public int Length => _buffer.Count;
		public int UnreadLength => Length - _readPosition;

		public UniCastPacket(ActorType sender)
		{
			Write(BitConverter.GetBytes((int)sender));
		}

		public UniCastPacket(ActorType sender, int id)
		{
			Write(BitConverter.GetBytes((int)sender));
			Write(BitConverter.GetBytes(id));
		}

		public UniCastPacket(InitialSendingData data)
		{
			Write(data.GetRawBytes());
		}

		public UniCastPacket(byte[] data)
		{
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

		public void Write(string text)
		{
			_buffer.AddRange(text.Length.ToByteArray());
			_buffer.AddRange(text.ToByteArray());
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
			try
			{
				return Read(4, moveReadPos).ToInt32();
			}
			catch
			{
				throw new Exception("Could not read value of type 'int'!");
			}
		}

		public string ReadString(bool moveReadPos = true)
		{
			try
			{
				int length = ReadInt(); // Get the length of the string
				string value = Encoding.ASCII.GetString(_readableBuffer, _readPosition, length); // Convert the bytes to a string
				if (moveReadPos && value.Length > 0)
				{
					// If _moveReadPos is true string is not empty
					_readPosition += length; // Increase readPos by the length of the string
				}
				return value; // Return the string
			}
			catch
			{
				throw new Exception("Could not read value of type 'string'!");
			}
		}

		public T Read<T>(bool moveReadPos = true) where T : IBasePacketDataWrapper, new()
		{
			try
			{
				T data = new T();
				data.SetTypedData(Read(data.RawDataLength, moveReadPos));
				return data;
			}
			catch
			{
				throw new Exception($"Could not read value of type '{typeof(T)}'!");
			}
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

		public void Reset(int byteLength)
		{
			byteLength = Math.Abs(byteLength);
			_readPosition -= byteLength; // "Unread" the last N bytes
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
