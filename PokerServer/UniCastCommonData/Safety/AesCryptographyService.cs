using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace UniCastCommonData.Secure
{
	public class AesCryptographyService
	{
		private static byte[] key = { 123, 217, 19, 11, 24, 26, 85, 45, 114, 184, 27, 162, 37, 112, 222, 209, 241, 24, 175, 144, 173, 53, 196, 29, 24, 26, 17, 218, 131, 236, 53, 209 };

		// a hardcoded IV should not be used for production AES-CBC code
		// IVs should be unpredictable per ciphertext
		private static byte[] vector = { 146, 64, 191, 111, 23, 3, 113, 119, 231, 121, 221, 112, 79, 32, 114, 156 };

		private ICryptoTransform encryptor, decryptor;
		private UTF8Encoding encoder;

		public AesCryptographyService()
		{
			RijndaelManaged rm = new RijndaelManaged();
			encryptor = rm.CreateEncryptor(key, vector);
			decryptor = rm.CreateDecryptor(key, vector);
			encoder = new UTF8Encoding();
		}

		public byte[] Encrypt(byte[] buffer)
		{
			return Transform(buffer, encryptor);
		}

		public byte[] Decrypt(byte[] buffer)
		{
			return Transform(buffer, decryptor);
		}

		protected byte[] Transform(byte[] buffer, ICryptoTransform transform)
		{
			MemoryStream stream = new MemoryStream();
			using (CryptoStream cs = new CryptoStream(stream, transform, CryptoStreamMode.Write))
			{
				cs.Write(buffer, 0, buffer.Length);
			}
			return stream.ToArray();
		}
	}
}
