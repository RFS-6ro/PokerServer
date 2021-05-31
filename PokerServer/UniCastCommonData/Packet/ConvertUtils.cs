using System;

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
	}
}
