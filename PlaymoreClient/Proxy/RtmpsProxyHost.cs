using PlaymoreClient.Messaging;
using PlaymoreClient.Util;
using FluorineFx.Messaging.Api.Event;
using FluorineFx.Messaging.Rtmp.Event;
using FluorineFx.Messaging.Rtmp.Service;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace PlaymoreClient.Proxy
{
	public class RtmpsProxyHost : SecureProxyHost, IMessageProcessor
	{
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
					this.Connected(this, new EventArgs());
				}
			}
		}

		public RtmpsProxyHost(int srcport, string remote, int remoteport, X509Certificate cert) : base(srcport, remote, remoteport, cert)
		{
		}

		public FluorineFx.Messaging.Rtmp.Event.Notify Call(object msg)
		{
			FlexInvoke flexInvoke = new FlexInvoke();
			flexInvoke.EventType = EventType.SERVICE_CALL;
			object[] objArray = new object[] { msg };
			flexInvoke.ServiceCall = new PendingCall(null, objArray);
			return this.CallWithInvoke(flexInvoke);
		}

		public FluorineFx.Messaging.Rtmp.Event.Notify CallWithInvoke(FluorineFx.Messaging.Rtmp.Event.Notify notify)
		{
			FluorineFx.Messaging.Rtmp.Event.Notify notify1;
			RtmpsProxyClient item = null;
			lock (base.Clients)
			{
				if (base.Clients.Count >= 1)
				{
					item = (RtmpsProxyClient)base.Clients[0];
					return item.Call(notify);
				}
				else
				{
					notify1 = null;
				}
			}
			return notify1;
		}

		public void ChangeRemote(string domain, X509Certificate cert)
		{
			base.RemoteAddress = domain;
			base.Certificate = cert;
		}

		public override ProxyClient NewClient(TcpClient tcp)
		{
			return new RtmpsProxyClient(this, tcp);
		}

		public virtual void OnCall(object sender, FluorineFx.Messaging.Rtmp.Event.Notify call, FluorineFx.Messaging.Rtmp.Event.Notify result)
		{
			if (this.CallResult != null)
			{
				this.CallResult(sender, call, result);
			}
			this.OnProcessResults(sender, result);
		}

		public override void OnConnect(ProxyClient sender)
		{
			this.IsConnected = true;
			base.OnConnect(sender);
		}

		public override void OnException(ProxyClient sender, Exception ex)
		{
			this.IsConnected = false;
			base.OnException(sender, ex);
		}

		public virtual void OnNotify(object sender, FluorineFx.Messaging.Rtmp.Event.Notify notify)
		{
			if (this.Notify != null)
			{
				this.Notify(sender, notify);
			}
			this.OnProcessResults(this, notify);
		}

		public virtual void OnProcessObject(object sender, object obj, long timestamp)
		{
			if (this.ProcessObject != null)
			{
                this.ProcessObject(sender, obj, timestamp);
			}
		}

		public virtual void OnProcessResults(object sender, FluorineFx.Messaging.Rtmp.Event.Notify results)
		{
			foreach (Tuple<object, long> body in RtmpUtil.GetBodies(results))
			{
				this.OnProcessObject(this, body.Item1, body.Item2);
			}
		}

		public event CallHandler CallResult;

		public event EventHandler Connected;

		public event NotifyHandler Notify;

		public event ProcessObjectHandler ProcessObject;
	}
}