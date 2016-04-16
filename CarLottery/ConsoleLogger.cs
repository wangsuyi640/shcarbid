using System;

namespace CarBidWebClient
{
	public class ConsoleLogger : LogHandler
	{
		private static readonly ConsoleLogger _instance = new ConsoleLogger();

		public static ConsoleLogger GetInstance()
		{
			return ConsoleLogger._instance;
		}

		public void AppendRaw(string log, int level)
		{
			Console.Write(log);
		}

		public void Flush()
		{
		}

		public void Close()
		{
		}
	}
}
