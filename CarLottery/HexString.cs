using System;
using System.Text;

namespace CarBidWebClient
{
	public class HexString
	{
		public const char NO_SPLIT_CHAR = '\0';

		private static readonly char[] hexChars = new char[]
		{
			'0',
			'1',
			'2',
			'3',
			'4',
			'5',
			'6',
			'7',
			'8',
			'9',
			'A',
			'B',
			'C',
			'D',
			'E',
			'F'
		};

		public static bool IsHexChar(char c)
		{
			return (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');
		}

		public static byte HexToDigit(char c)
		{
			if (c >= '0' && c <= '9')
			{
				return (byte)(c - '0');
			}
			if (c >= 'a' && c <= 'f')
			{
				return (byte)(c - 'a' + '\n');
			}
			if (c >= 'A' && c <= 'F')
			{
				return (byte)(c - 'A' + '\n');
			}
			throw new Exception("无效的十六进制字符！");
		}

		public static byte[] GetBytes(object data)
		{
			if (data == null || data is byte[])
			{
				return data as byte[];
			}
			if (data is decimal)
			{
				return new byte[]
				{
					(byte)data
				};
			}
			if (!(data is string))
			{
				throw new InvalidCastException();
			}
			string text = data as string;
			byte[] array = new byte[text.Length / 2 + 1];
			int num = 0;
			byte b = 0;
			bool flag = true;
			string text2 = text;
			for (int i = 0; i < text2.Length; i++)
			{
				char c = text2[i];
				if (HexString.IsHexChar(c))
				{
					if (flag)
					{
						b = HexString.HexToDigit(c);
						flag = false;
					}
					else
					{
						array[num++] = (byte)((int)b << 4 | (int)HexString.HexToDigit(c));
						b = 0;
						flag = true;
					}
				}
				else if (!flag)
				{
					array[num++] = b;
					b = 0;
					flag = true;
				}
			}
			if (!flag)
			{
				array[num++] = b;
			}
			if (array.Length != num)
			{
				byte[] array2 = new byte[num];
				Array.Copy(array, array2, num);
				array = array2;
			}
			return array;
		}

		public static string BytesToHex(byte[] ba, int offset, int length, char split)
		{
			if (ba == null || ba.Length == 0 || length == 0)
			{
				return string.Empty;
			}
			if (offset < 0 || offset >= ba.Length || offset + length > ba.Length)
			{
				throw new IndexOutOfRangeException();
			}
			StringBuilder stringBuilder;
			if (split == '\0')
			{
				stringBuilder = new StringBuilder(ba.Length * 2);
			}
			else
			{
				stringBuilder = new StringBuilder(ba.Length * 3);
			}
			for (int i = offset; i < offset + length; i++)
			{
				stringBuilder.Append(HexString.hexChars[ba[i] >> 4 & 15]);
				stringBuilder.Append(HexString.hexChars[(int)(ba[i] & 15)]);
				if (split != '\0' && i - offset + 1 != length)
				{
					stringBuilder.Append(split);
				}
			}
			return stringBuilder.ToString();
		}

		public static string GetString(byte[] data)
		{
			if (data == null)
			{
				return string.Empty;
			}
			return HexString.BytesToHex(data, 0, data.Length, ' ');
		}

		public static string GetStringNoSplit(byte[] data)
		{
			if (data == null)
			{
				return string.Empty;
			}
			return HexString.BytesToHex(data, 0, data.Length, '\0');
		}
	}
}
