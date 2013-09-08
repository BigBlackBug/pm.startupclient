using NotMissing.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;

namespace PlaymoreClient.Proxy
{
	public class ProxyClient : IDisposable
	{
		private const int BufferSize = 65535;

		protected bool _disposed;

		protected ProcessQueue<Tuple<byte[], int, int>> SourceQueue = new ProcessQueue<Tuple<byte[], int, int>>();

		protected ProcessQueue<Tuple<byte[], int, int>> RemoteQueue = new ProcessQueue<Tuple<byte[], int, int>>();

        public ProxyClient() { }
		public IProxyHost Host
		{
			get;
			protected set;
		}

		private byte[] RemoteBuffer
		{
			get;
			set;
		}

		public Stream RemoteStream
		{
			get;
			protected set;
		}

		public TcpClient RemoteTcp
		{
			get;
			protected set;
		}

		private byte[] SourceBuffer
		{
			get;
			set;
		}

		public Stream SourceStream
		{
			get;
			protected set;
		}

		public TcpClient SourceTcp
		{
			get;
			protected set;
		}

		public ProxyClient(IProxyHost host, TcpClient src)
		{
			this.Host = host;
			this.SourceTcp = src;
			this.SourceBuffer = new byte[65535];
			this.RemoteBuffer = new byte[65535];
			this.RemoteTcp = new TcpClient();
			this.SourceQueue.Process += new EventHandler<ProcessQueueEventArgs<Tuple<byte[], int, int>>>(this.SourceQueue_Process);
			this.RemoteQueue.Process += new EventHandler<ProcessQueueEventArgs<Tuple<byte[], int, int>>>(this.RemoteQueue_Process);
		}

		protected virtual void BeginReceive(IAsyncResult ar)
		{
			byte[] numArray;
			try
			{
				Stream asyncState = (Stream)ar.AsyncState;
				int num = asyncState.EndRead(ar);
				if (num == 0)
				{
					throw new EndOfStreamException(string.Format("{0} socket closed", (asyncState == this.SourceStream ? "Source" : "Remote")));
				}
				if (asyncState != this.SourceStream)
				{
					this.OnReceive(this.RemoteBuffer, 0, num);
				}
				else
				{
					this.OnSend(this.SourceBuffer, 0, num);
				}
				Stream stream = asyncState;
				numArray = (asyncState == this.SourceStream ? this.SourceBuffer : this.RemoteBuffer);
				ProxyClient proxyClient = this;
				stream.BeginRead(numArray, 0, 65535, new AsyncCallback(proxyClient.BeginReceive), asyncState);
			}
			catch (Exception exception)
			{
				this.Host.OnException(this, exception);
			}
		}

		protected virtual void ConnectRemote(string remote, int remoteport)
		{
			this.RemoteTcp.Connect(remote, remoteport);
			this.SourceStream = this.Host.GetStream(this.SourceTcp);
			this.RemoteStream = this.Host.GetStream(this.RemoteTcp);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && !this._disposed)
			{
				this._disposed = true;
				this.Stop();
				this.SourceQueue.Dispose();
				this.RemoteQueue.Dispose();
			}
		}

		~ProxyClient()
		{
			this.Dispose(false);
		}

		protected virtual void OnReceive(byte[] buffer, int idx, int len)
		{
			this.Host.OnReceive(this, buffer, idx, len);
			this.SourceQueue.Enqueue(Tuple.Create<byte[], int, int>(buffer, idx, len));
		}

		protected virtual void OnSend(byte[] buffer, int idx, int len)
		{
			this.Host.OnSend(this, buffer, idx, len);
			this.RemoteQueue.Enqueue(Tuple.Create<byte[], int, int>(buffer, idx, len));
		}

		private void RemoteQueue_Process(object sender, ProcessQueueEventArgs<Tuple<byte[], int, int>> e)
		{
			IAsyncResult asyncResult = this.RemoteStream.BeginWrite(e.Item.Item1, e.Item.Item2, e.Item.Item3, null, null);
			using (asyncResult.AsyncWaitHandle)
			{
				if (asyncResult.AsyncWaitHandle.WaitOne(-1))
				{
					this.RemoteStream.EndWrite(asyncResult);
				}
			}
		}

		private void SourceQueue_Process(object sender, ProcessQueueEventArgs<Tuple<byte[], int, int>> e)
		{
			IAsyncResult asyncResult = this.SourceStream.BeginWrite(e.Item.Item1, e.Item.Item2, e.Item.Item3, null, null);
			using (asyncResult.AsyncWaitHandle)
			{
				if (asyncResult.AsyncWaitHandle.WaitOne(-1))
				{
					this.SourceStream.EndWrite(asyncResult);
				}
			}
		}

		public virtual void Start(string remote, int remoteport)
		{
			try
			{
				this.ConnectRemote(remote, remoteport);
				this.Host.OnConnect(this);
				ProxyClient proxyClient = this;
				this.SourceStream.BeginRead(this.SourceBuffer, 0, 65535, new AsyncCallback(proxyClient.BeginReceive), this.SourceStream);
				ProxyClient proxyClient1 = this;
				this.RemoteStream.BeginRead(this.RemoteBuffer, 0, 65535, new AsyncCallback(proxyClient1.BeginReceive), this.RemoteStream);
			}
			catch (Exception exception)
			{
				this.Host.OnException(this, exception);
			}
		}

		public virtual void Stop()
		{
			Action<Action> action = (Action act) => {
				try
				{
					act();
				}
				catch (Exception exception)
				{
					PlaymoreClient.Gui.MainForm.LOGGER.Info(exception);
				}
			};
			action(new Action(this.SourceTcp.Close));
			action(new Action(this.RemoteTcp.Close));
		}
	}
}