using NotMissing.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace PlaymoreClient.Proxy
{
	public class ProxyHost : IProxyHost, IDisposable
    {
		public List<ProxyClient> Clients
		{
			get;
			protected set;
		}

		public bool IsListening
		{
			get
			{
				return this.Listener != null;
			}
		}

		public TcpListener Listener
		{
			get;
			protected set;
		}

		public string RemoteAddress
		{
			get;
			set;
		}

		public int RemotePort
		{
			get;
			set;
		}

		public IPAddress SourceAddress
		{
			get;
			set;
		}

		public int SourcePort
		{
			get;
			set;
		}

		public ProxyHost(int srcport, string remote, int remoteport) : this(IPAddress.Loopback, srcport, remote, remoteport)
		{
		}

		public ProxyHost(IPAddress src, int srcport, string remote, int remoteport)
		{
			this.Clients = new List<ProxyClient>();
			this.SourceAddress = src;
			this.SourcePort = srcport;
			this.RemoteAddress = remote;
			this.RemotePort = remoteport;
			this.Listener = null;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.Stop();
			}
		}

		~ProxyHost()
		{
			this.Dispose(false);
		}

		public virtual Stream GetStream(TcpClient tcp)
		{
			return tcp.GetStream();
		}

		public virtual ProxyClient NewClient(TcpClient tcp)
		{
			return new ProxyClient(this, tcp);
		}

		protected virtual void OnAccept(IAsyncResult ar)
		{
            PlaymoreClient.Gui.MainForm.LOGGER.Info("accepted connection");
			ProxyClient proxyClient = null;
			try
			{
				if (this.IsListening)
				{
					proxyClient = this.NewClient(this.Listener.EndAcceptTcpClient(ar));
					ProxyHost proxyHost = this;
					this.Listener.BeginAcceptTcpClient(new AsyncCallback(proxyHost.OnAccept), null);
					lock (this.Clients)
					{
						this.Clients.Add(proxyClient);
					}
					proxyClient.Start(this.RemoteAddress, this.RemotePort);
					if (proxyClient.SourceTcp.Client != null)
					{
						PlaymoreClient.Gui.MainForm.LOGGER.Info(string.Format("Client {0} connected", proxyClient.SourceTcp.Client.RemoteEndPoint));
					}
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				if (proxyClient != null)
				{
					this.OnException(proxyClient, exception);
				}
				else if (!(exception is ObjectDisposedException))
				{
					PlaymoreClient.Gui.MainForm.LOGGER.Error(exception);
				}
			}
		}

		public virtual void OnConnect(ProxyClient sender)
		{
		}

		public virtual void OnException(ProxyClient sender, Exception ex)
		{
			lock (this.Clients)
			{
				int num = this.Clients.IndexOf(sender);
				if (num != -1)
				{
					this.Clients.RemoveAt(num);
				}
			}
			sender.Dispose();
			PlaymoreClient.Gui.MainForm.LOGGER.Debug(ex);
		}

		public virtual void OnReceive(ProxyClient sender, byte[] buffer, int idx, int len)
		{
		}

		public virtual void OnSend(ProxyClient sender, byte[] buffer, int idx, int len)
		{
		}

		public virtual void Start()
		{
			if (!this.IsListening)
			{
                PlaymoreClient.Gui.MainForm.LOGGER.Info("started connection");
				this.Listener = new TcpListener(this.SourceAddress, this.SourcePort);
				this.Listener.Start();
				ProxyHost proxyHost = this;
				this.Listener.BeginAcceptTcpClient(new AsyncCallback(proxyHost.OnAccept), null);
			}
		}

		public virtual void Stop()
		{
			if (this.IsListening)
			{
				this.Listener.Stop();
				this.Listener = null;
				lock (this.Clients)
				{
					for (int i = 0; i < this.Clients.Count; i++)
					{
						this.Clients[i].Dispose();
					}
					this.Clients.Clear();
				}
			}
		}
	}
}