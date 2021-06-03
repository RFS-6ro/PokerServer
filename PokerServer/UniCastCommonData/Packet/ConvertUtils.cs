using System;
using System.Linq;
using System.Text;

namespace UniCastCommonData
{
	public static class ConvertUtils
	{
		public static int ToInt32(this byte[] buffer)
		{
			if (buffer.Length != 4)
			{
				throw new Exception("Types does not match");
			}

			return BitConverter.ToInt32(buffer, 0);
		}

		public static byte[] ToByteArray(this int value)
		{
			return BitConverter.GetBytes(value);
		}

		public static string ToString(this byte[] buffer)
		{
			if (buffer.Length <= 4)
			{
				return string.Empty;
			}

			int length = new byte[] { buffer[0], buffer[1], buffer[2], buffer[3] }.ToInt32();

			return Encoding.ASCII.GetString(buffer, 4, length);
		}

		public static byte[] ToByteArray(this string value)
		{
			return Encoding.ASCII.GetBytes(value);
		}
	}
}
