using NotMissing.Logging;
using System;
using System.Diagnostics;

namespace PlaymoreClient.Util
{
	public class SimpleLogTimer : IDisposable
	{
		private readonly Stopwatch m_Watch;

		private string m_Message;

		private Levels m_Level;

		protected SimpleLogTimer(Levels level, string message)
		{
			this.m_Level = level;
			this.m_Watch = Stopwatch.StartNew();
			this.m_Message = message;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.m_Watch.Stop();
				PlaymoreClient.Gui.MainForm.LOGGER.Info(this.m_Level +" "+ string.Format("[Timing] {0} in {1}ms", this.m_Message, this.m_Watch.ElapsedMilliseconds));
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		~SimpleLogTimer()
		{
			this.Dispose(false);
		}

		public static SimpleLogTimer Start(Levels level, string message)
		{
			return new SimpleLogTimer(level, message);
		}

		public static SimpleLogTimer Start(string message)
		{
			return SimpleLogTimer.Start(Levels.Trace, message);
		}
	}
}