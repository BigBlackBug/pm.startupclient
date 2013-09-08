using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace PlaymoreClient.Util
{
	public class ProcessMonitor
	{
		public Thread CheckThread
		{
			get;
			protected set;
		}

		public Process CurrentProcess
		{
			get;
			protected set;
		}

		public string[] ProcessNames
		{
			get;
			protected set;
		}

		public ProcessMonitor(string[] processnames)
		{
			this.ProcessNames = processnames;
			Thread thread = new Thread(new ThreadStart(this.CheckLoop));
			thread.IsBackground = true;
			this.CheckThread = thread;
		}

		protected void CheckLoop()
		{
			while (this.CheckThread != null)
			{
				if (this.CurrentProcess == null || this.CurrentProcess.HasExited)
				{
					int num = 0;
					while (num < (int)this.ProcessNames.Length)
					{
						this.CurrentProcess = Process.GetProcessesByName(this.ProcessNames[num]).FirstOrDefault<Process>();
						if (this.CurrentProcess == null)
						{
							num++;
						}
						else
						{
							if (this.ProcessFound == null)
							{
								break;
							}
							this.ProcessFound(this, new ProcessMonitor.ProcessEventArgs(this.CurrentProcess));
							break;
						}
					}
				}
				Thread.Sleep(500);
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool dispose)
		{
			if (dispose)
			{
				this.CheckThread = null;
			}
		}

		~ProcessMonitor()
		{
			this.Dispose(false);
		}

		public void Start()
		{
			if (!this.CheckThread.IsAlive)
			{
				this.CheckThread.Start();
			}
		}

		public void Stop()
		{
			this.CheckThread = null;
		}

		public event EventHandler<ProcessMonitor.ProcessEventArgs> ProcessFound;

		public class ProcessEventArgs : EventArgs
		{
			public Process Process
			{
				get;
				set;
			}

			public ProcessEventArgs(Process process)
			{
				this.Process = process;
			}
		}
	}
}