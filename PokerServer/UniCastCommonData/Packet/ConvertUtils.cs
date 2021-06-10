using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniCastCommonData
{
	public static class ConvertUtils
	{
		public static int ToInt32(this byte[] buffer, int index = 0)
		{
			if (buffer.Length - index < 4)
			{
				throw new Exception("Types does not match");
			}

			return BitConverter.ToInt32(buffer, index);
		}

		public static byte[] ToByteArray(this int value)
		{
			return BitConverter.GetBytes(value);
		}

		public static bool ToBoolean(this byte[] buffer, int index = 0)
		{
			if (buffer.Length - index < 4)
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
	}
}
