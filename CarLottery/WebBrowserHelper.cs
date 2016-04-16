using System;
using System.Runtime.InteropServices;
using System.Text;

namespace CarBidWebClient
{
	public class WebBrowserHelper
	{
		private const int S_OK = 0;

		private const int URLMON_OPTION_USERAGENT = 268435457;

		private const int URLMON_OPTION_USERAGENT_REFRESH = 268435458;

		private const int URLMON_OPTION_URL_ENCODING = 268435460;

		public static string UserAgent
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder(1024);
				int num = 0;
				WebBrowserHelper.UrlMkGetSessionOption(268435457, stringBuilder, stringBuilder.Capacity + 1, ref num, 0);
				return stringBuilder.ToString();
			}
			set
			{
				if (value == null)
				{
					WebBrowserHelper.UrlMkSetSessionOption(268435458, null, 0, 0);
					return;
				}
				WebBrowserHelper.UrlMkSetSessionOption(268435457, value, value.Length, 0);
			}
		}

		[DllImport("urlmon.dll", CharSet = CharSet.Ansi)]
		private static extern int UrlMkGetSessionOption(int dwOption, StringBuilder pBuffer, int dwBufferLength, ref int pdwBufferLengthOut, int dwReserved);

		[DllImport("urlmon.dll", CharSet = CharSet.Ansi)]
		private static extern int UrlMkSetSessionOption(int dwOption, string pBuffer, int dwBufferLength, int dwReserved);
	}
}
