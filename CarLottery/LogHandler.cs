using System;

namespace CarBidWebClient
{
	public interface LogHandler
	{
		void AppendRaw(string log, int level);

		void Flush();

		void Close();
	}
}
