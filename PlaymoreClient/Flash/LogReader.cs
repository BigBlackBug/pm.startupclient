using NotMissing.Logging;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace PlaymoreClient.Flash
{
	public class LogReader : IDisposable
	{
		private const string LogMatch = "^\\d+/\\d+/\\d+ \\d+:\\d+:\\d+\\.\\d+ \\[\\w+\\]";

		private const string ObjectMatch = "\\(([^\\)]+)\\)#\\d+$";

		private StreamReader Reader
		{
			get;
			set;
		}

		public LogReader(Stream stream) : this(new StreamReader(stream))
		{
		}

		public LogReader(StreamReader reader)
		{
			this.Reader = reader;
		}

		protected virtual void Dispose(bool disp)
		{
			if (disp && this.Reader != null)
			{
				this.Reader.Dispose();
				this.Reader = null;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		~LogReader()
		{
			this.Dispose(false);
		}

		public object Read()
		{
			object obj;
			string str = this.Reader.ReadLine();
			if (str == null)
			{
				throw new EndOfStreamException();
			}
			Match match = Regex.Match(str, "^\\d+/\\d+/\\d+ \\d+:\\d+:\\d+\\.\\d+ \\[\\w+\\]");
			if (match.Success)
			{
				try
				{
					match = Regex.Match(str, "\\(([^\\)]+)\\)#\\d+$");
					if (!match.Success)
					{
						obj = str;
					}
					else
					{
						FlashObject value = FlashSerializer.Deserialize(this.Reader);
						value.Name = match.Groups[1].Value;
						obj = value;
					}
				}
				catch (EndOfStreamException endOfStreamException)
				{
					throw;
				}
				catch (Exception exception)
				{
					PlaymoreClient.Gui.MainForm.LOGGER.Error(exception);
					return null;
				}
				return obj;
			}
			return null;
		}
	}
}