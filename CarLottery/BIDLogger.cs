using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CarBidWebClient
{
	public class BIDLogger
	{
		public const int DEBUG = 0;

		public const int INFO = 1;

		public const int WARN = 2;

		public const int ERROR = 3;

		public const int NONE = 2147483646;

		public const int FORCE = 2147483647;

		private List<LogHandler> _handlerList = new List<LogHandler>();

		private string _name;

		private int _level;

		private bool _logCaller = true;

		private static Dictionary<string, BIDLogger> _cache = new Dictionary<string, BIDLogger>();

		public string Name
		{
			get
			{
				return this._name;
			}
		}

		public bool LogCaller
		{
			get
			{
				return this._logCaller;
			}
			set
			{
				this._logCaller = value;
			}
		}

		public int Level
		{
			get
			{
				return this._level;
			}
			set
			{
				this._level = value;
			}
		}

		private BIDLogger(string name)
		{
			this._name = ((name == null) ? "Log" : name);
		}

		public static BIDLogger GetLogger(string name = null)
		{
			if (name == null)
			{
				name = "Default";
			}
			BIDLogger bIDLogger = null;
			BIDLogger._cache.TryGetValue(name, out bIDLogger);
			if (bIDLogger == null)
			{
				lock (BIDLogger._cache)
				{
					bIDLogger = new BIDLogger(name);
					BIDLogger._cache[name] = bIDLogger;
				}
			}
			return bIDLogger;
		}

		private string GetLevelString(int level)
		{
			switch (level)
			{
			case 0:
				return "调试";
			case 1:
				return "信息";
			case 2:
				return "警告";
			case 3:
				return "错误";
			default:
				if (level != 2147483647)
				{
					return string.Format("级别 {0}", level);
				}
				return "强制";
			}
		}

		public void Append(string str, int level = 1)
		{
			if (str == null || level < this._level)
			{
				return;
			}
			string str2;
			if (!this._logCaller)
			{
				str2 = string.Format("\r\n================================================================================\r\n{1} {0}: {3}", new object[]
				{
					DateTime.Now.ToString("HH:mm:ss.fff"),
					this.GetLevelString(level),
					this._name,
					str
				});
			}
			else
			{
				StackFrame stackFrame = new StackFrame(1);
				str2 = string.Format("\r\n================================================================================\r\n{1} {0} [{3}]: {4}", new object[]
				{
					DateTime.Now.ToString("HH:mm:ss.fff"),
					this.GetLevelString(level),
					this._name,
					stackFrame.GetMethod().Name,
					str
				});
			}
			this.AppendRaw(str2, level);
		}

		public void Append(string title, string data, int level = 1)
		{
			this.Append(string.Format("{0}\r\n{1}", title, data), level);
		}

		public void AppendRaw(string str, int level = 2147483647)
		{
			if (str == null)
			{
				return;
			}
			lock (this)
			{
				foreach (LogHandler current in this._handlerList)
				{
					try
					{
						current.AppendRaw(str, level);
					}
					catch
					{
					}
				}
			}
		}

		public void AddHandler(LogHandler l)
		{
			if (l == null)
			{
				return;
			}
			lock (this)
			{
				if (!this._handlerList.Contains(l))
				{
					this._handlerList.Add(l);
				}
			}
		}

		public void RemoveHandler(LogHandler l)
		{
			if (l == null)
			{
				return;
			}
			lock (this)
			{
				this._handlerList.Remove(l);
			}
		}
	}
}
