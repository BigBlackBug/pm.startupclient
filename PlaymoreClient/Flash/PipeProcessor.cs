using NotMissing.Logging;
using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;

namespace PlaymoreClient.Flash
{
	public class PipeProcessor : IFlashProcessor, IDisposable
	{
		private Thread RecvThread;

		private readonly string PipeName;

		private bool isconnected;

		public bool IsConnected
		{
			get
			{
				return this.isconnected;
			}
			protected set
			{
				bool flag = this.isconnected;
				this.isconnected = value;
				if (flag != value && this.Connected != null)
				{
					this.Connected(this);
				}
			}
		}

		public PipeProcessor(string pipename)
		{
			this.PipeName = pipename;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.RecvThread = null;
				this.ProcessObject = null;
				this.ProcessLine = null;
				this.Connected = null;
			}
		}

		protected virtual void DoProcessLine(string str)
		{
			if (this.ProcessLine != null)
			{
				this.ProcessLine(str);
			}
		}

		protected virtual void DoProcessObject(FlashObject obj)
		{
			if (this.ProcessObject != null)
			{
				this.ProcessObject(obj);
			}
		}

		~PipeProcessor()
		{
			this.Dispose(false);
		}

		protected virtual void RecvLoop()
		{
			while (this.RecvThread != null)
			{
				try
				{
					try
					{
						using (NamedPipeClientStream namedPipeClientStream = new NamedPipeClientStream(this.PipeName))
						{
							using (LogReader logReader = new LogReader(namedPipeClientStream))
							{
								namedPipeClientStream.Connect(0);
								this.IsConnected = namedPipeClientStream.IsConnected;
								while (namedPipeClientStream.IsConnected)
								{
									object obj = logReader.Read();
									if (obj == null)
									{
										continue;
									}
									if (!(obj is FlashObject))
									{
										if (!(obj is string))
										{
											continue;
										}
										this.DoProcessLine((string)obj);
									}
									else
									{
										this.DoProcessObject((FlashObject)obj);
									}
								}
							}
						}
					}
					catch (IOException oException)
					{
					}
					catch (TimeoutException timeoutException)
					{
					}
					catch (Exception exception)
					{
						PlaymoreClient.Gui.MainForm.LOGGER.Error(exception);
					}
				}
				finally
				{
					this.IsConnected = false;
				}
				Thread.Sleep(500);
			}
		}

		public virtual void Start()
		{
			if (this.RecvThread == null)
			{
				PipeProcessor pipeProcessor = this;
				Thread thread = new Thread(new ThreadStart(pipeProcessor.RecvLoop));
				thread.IsBackground = true;
				this.RecvThread = thread;
				this.RecvThread.Start();
			}
		}

		public virtual void Stop()
		{
			this.RecvThread = null;
		}

		public event Action<object> Connected;

		public event ProcessLineD ProcessLine;

		public event ProcessObjectD ProcessObject;
	}
}