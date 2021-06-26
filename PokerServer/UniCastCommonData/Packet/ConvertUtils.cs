using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniCastCommonData.Handlers.Convert
{
	public static class ConvertUtils
	{
		public static int ToInt32(this byte[] buffer, int index = 0)
		{
			if (buffer.Length - index < 4)
			{
				throw new Exception("Types does not match");
			}

			//ReadOnlySpan<byte> bytes = new ReadOnlySpan<byte>(buffer, index, 4);
			//return BinaryPrimitives.ReadInt32BigEndian(bytes);
			return BitConverter.ToInt32(buffer, index);
		}

		public static byte[] ToByteArray(this int value)
		{
			return BitConverter.GetBytes(value);
			//byte[] bytes = new byte[4];
			//BinaryPrimitives.WriteInt32BigEndian(bytes, value);
			//return bytes;
		}

		public static bool ToBoolean(this byte[] buffer, int index = 0)
		{
			if (buffer.Length - index < 1)
			{
				throw new Exception("Types does not match");
			}

			return BitConverter.ToBoolean(buffer, index);
		}

		public static byte[] ToByteArray(this bool value)
		{
			return BitConverter.GetBytes(value);
		}

		public static string ToString(this byte[] buffer, int index = 0)
		{
			if (buffer.Length - index <= 4)
			{
				return string.Empty;
			}

			int length = buffer.ToInt32(index);

			return Encoding.ASCII.GetString(buffer, index + 4, length);
		}

		public static byte[] ToByteArray(this string value)
		{
			var data = value.Length.ToByteArray().ToList();
			data.AddRange(Encoding.ASCII.GetBytes(value));
			return data.ToArray();
		}

		public static Guid ToGuid(this byte[] array, int index)
		{
			List<byte> data = new List<byte>();
			for (int i = index; i < index + 16; i++)
			{
				data.Add(array[i]);
			}
			return new Guid(data.ToArray());
		}

		public static int ReadInt(this UniCastPacket packet, bool moveReadPos = true)
		{
			try
			{
				return packet.Read(4, moveReadPos).ToInt32();
			}
			catch
			{
				throw new Exception("Could not read value of type 'int'!");
			}
		}
	}
}
