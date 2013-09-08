using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace PlaymoreClient.Util
{
	public static class Compression
	{
		public static byte[] CompressString(string str)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Compress))
				{
					byte[] bytes = Encoding.UTF8.GetBytes(str);
					gZipStream.Write(bytes, 0, (int)bytes.Length);
				}
				array = memoryStream.ToArray();
			}
			return array;
		}

		public static string DecompressString(byte[] data)
		{
			string str;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
				{
					gZipStream.Write(data, 0, (int)data.Length);
				}
				str = Encoding.UTF8.GetString(memoryStream.ToArray());
			}
			return str;
		}
	}
}