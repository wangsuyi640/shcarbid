using Microsoft.JScript;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace CarBidWebClient
{
	public class BIDWebUtils
	{
		public static string StrToJson(string str)
		{
			if (str.StartsWith("\""))
			{
				str = str.Substring(1);
			}
			if (str.EndsWith("\""))
			{
				str = str.Substring(0, str.Length - 1);
			}
			return Regex.Replace(str, "\\\\(.)", "$1");
		}

		public static string JsonToStr(string str)
		{
			str = Regex.Replace(str, "([\"\\\\])", "\\$1");
			return "\"" + str + "\"";
		}

		public static string Escape(string str)
		{
			return GlobalObject.escape(str);
		}

		public static string UnEscape(string str)
		{
			return GlobalObject.unescape(str);
		}

		public static string GetHTTPRequestID()
		{
			return ((long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds).ToString();
		}

		public static string MD5(string str)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(str);
			MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
			return HexString.GetStringNoSplit(mD5CryptoServiceProvider.ComputeHash(bytes)).ToLower();
		}
	}
}
