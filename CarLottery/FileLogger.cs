using System;
using System.IO;
using System.Text;

namespace CarBidWebClient
{
	public class FileLogger : LogHandler
	{
		private Encoding _encoding = Encoding.Default;

		private FileStream _stream;

		public Encoding Encoding
		{
			get
			{
				return this._encoding;
			}
			set
			{
				this._encoding = ((value == null) ? Encoding.Default : value);
			}
		}

		public FileLogger(string path)
		{
			this._stream = new FileStream(path, FileMode.Append);
		}

		public void AppendRaw(string log, int level)
		{
			if (this._stream != null && log != null)
			{
				lock (this)
				{
					byte[] bytes = this._encoding.GetBytes(log);
					this._stream.Write(bytes, 0, bytes.Length);
				}
			}
		}

		public void Flush()
		{
			if (this._stream != null)
			{
				this._stream.Flush();
			}
		}

		public void Close()
		{
			this._stream.Close();
			this._stream = null;
		}
	}
}
