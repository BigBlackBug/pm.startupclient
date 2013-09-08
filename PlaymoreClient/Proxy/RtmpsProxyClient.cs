using PlaymoreClient.Util;
using FluorineFx;
using FluorineFx.Messaging.Api.Service;
using FluorineFx.Messaging.Rtmp;
using FluorineFx.Messaging.Rtmp.Event;
using FluorineFx.Util;
using NotMissing.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;

namespace PlaymoreClient.Proxy
{
	public class RtmpsProxyClient : ProxyClient
	{
		private const bool encode = true;

		private const bool logtofiles = false;

		protected RtmpContext sourcecontext;

		protected RtmpContext remotecontext;

		protected ByteBuffer postbuffer;

		protected readonly ByteBuffer sendbuffer;

		protected readonly ByteBuffer receivebuffer;

		protected readonly List<Notify> InvokeList;

		protected List<CallResultWait> WaitInvokeList;

		protected readonly object WaitLock;

		protected readonly Dictionary<int, int> InvokeIds;

		protected AtomicInteger CurrentInvoke;

		public new RtmpsProxyHost Host
		{
			get;
			protected set;
		}

		public RtmpsProxyClient(IProxyHost host, TcpClient src): base(host, src)
        {
           
			RtmpContext rtmpContext = new RtmpContext(RtmpMode.Server);
			rtmpContext.ObjectEncoding = ObjectEncoding.AMF0;
			this.sourcecontext = rtmpContext;
			RtmpContext rtmpContext1 = new RtmpContext(RtmpMode.Client);
			rtmpContext1.ObjectEncoding = ObjectEncoding.AMF0;
			this.remotecontext = rtmpContext1;
			this.postbuffer = new ByteBuffer(new MemoryStream());
			this.sendbuffer = new ByteBuffer(new MemoryStream());
			this.receivebuffer = new ByteBuffer(new MemoryStream());
			this.InvokeList = new List<Notify>();
			this.WaitInvokeList = new List<CallResultWait>();
			this.WaitLock = new object();
			this.InvokeIds = new Dictionary<int, int>();
			this.CurrentInvoke = new AtomicInteger();
			
			if (!(host is RtmpsProxyHost))
			{
				throw new ArgumentException(string.Concat("Expected RtmpsProxyHost, got ", host.GetType()));
			}
			this.Host = (RtmpsProxyHost)host;
		}

		private RtmpsProxyClient():base(null, null)
		{
			RtmpContext rtmpContext = new RtmpContext(RtmpMode.Server);
			rtmpContext.ObjectEncoding = ObjectEncoding.AMF0;
			this.sourcecontext = rtmpContext;
			RtmpContext rtmpContext1 = new RtmpContext(RtmpMode.Client);
			rtmpContext1.ObjectEncoding = ObjectEncoding.AMF0;
			this.remotecontext = rtmpContext1;
			this.postbuffer = new ByteBuffer(new MemoryStream());
			this.sendbuffer = new ByteBuffer(new MemoryStream());
			this.receivebuffer = new ByteBuffer(new MemoryStream());
			this.InvokeList = new List<Notify>();
			this.WaitInvokeList = new List<CallResultWait>();
			this.WaitLock = new object();
			this.InvokeIds = new Dictionary<int, int>();
			this.CurrentInvoke = new AtomicInteger();
			
		}

		public Notify Call(Notify notify)
		{
			Notify notify1;
			CallResultWait callResultWait = new CallResultWait(notify, true);
			lock (this.WaitLock)
			{
				if (this.WaitInvokeList != null)
				{
					this.WaitInvokeList.Add(callResultWait);
					notify.InvokeId = this.CurrentInvoke.Increment();
					this.InternalSend(notify, false);
					callResultWait.Wait.WaitOne(-1);
					return callResultWait.Result;
				}
				else
				{
					notify1 = null;
				}
			}
			return notify1;
		}

		public RtmpPacket CreatePacket(Notify notify)
		{
			RtmpHeader rtmpHeader = new RtmpHeader();
			rtmpHeader.ChannelId = 3;
			rtmpHeader.DataType = notify.DataType;
			rtmpHeader.Timer = notify.Timestamp;
			return new RtmpPacket(rtmpHeader, notify);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				lock (this.WaitLock)
				{
					if (this.WaitInvokeList != null)
					{
						foreach (CallResultWait waitInvokeList in this.WaitInvokeList)
						{
							waitInvokeList.Wait.Set();
						}
						this.WaitInvokeList = null;
					}
				}
			}
			base.Dispose(disposing);
		}

		protected void InternalReceive(Notify notify)
		{
			int num;
			if (RtmpUtil.IsResult(notify))
			{
				int invokeId = notify.InvokeId;
				CallResultWait item = null;
				lock (this.WaitLock)
				{
					if (this.WaitInvokeList != null)
					{
						int num1 = this.WaitInvokeList.FindIndex((CallResultWait crw) => crw.Call.InvokeId == invokeId);
						if (num1 != -1)
						{
							item = this.WaitInvokeList[num1];
							this.WaitInvokeList.RemoveAt(num1);
						}
					}
				}
				if (item != null)
				{
					item.Result = notify;
					item.Wait.Set();
					if (!item.Blocking)
					{
						this.OnCall(item.Call, item.Result);
					}
					return;
				}
				lock (this.InvokeIds)
				{
					if (this.InvokeIds.TryGetValue(invokeId, out num))
					{
						this.InvokeIds.Remove(invokeId);
					}
					else
					{
						PlaymoreClient.Gui.MainForm.LOGGER.Error(string.Format("Call id not found for {0}", notify));
						return;
					}
				}
				notify.InvokeId = num;
			}
			ByteBuffer byteBuffer = RtmpProtocolEncoder.Encode(this.remotecontext, this.CreatePacket(notify));
			if (byteBuffer == null)
			{
				PlaymoreClient.Gui.MainForm.LOGGER.Fatal(string.Concat("Unable to encode ", notify));
				return;
			}
			byte[] array = byteBuffer.ToArray();
			base.OnReceive(array, 0, (int)array.Length);
		}

		protected void InternalSend(Notify notify, bool overwriteid)
		{
			if (overwriteid)
			{
				int num = this.CurrentInvoke.Increment();
				lock (this.InvokeIds)
				{
					this.InvokeIds.Add(num, notify.InvokeId);
				}
				notify.InvokeId = num;
			}
			ByteBuffer byteBuffer = RtmpProtocolEncoder.Encode(this.sourcecontext, this.CreatePacket(notify));
			if (byteBuffer == null)
			{
				PlaymoreClient.Gui.MainForm.LOGGER.Fatal(string.Concat("Unable to encode ", notify));
				return;
			}
			byte[] array = byteBuffer.ToArray();
			base.OnSend(array, 0, (int)array.Length);
		}

		protected virtual void OnCall(Notify call, Notify result)
		{
			this.Host.OnCall(this, call, result);
		}

		protected virtual void OnNotify(Notify notify)
		{
			this.Host.OnNotify(this, notify);
		}

		protected override void OnReceive(byte[] buffer, int idx, int len)
		{
            Console.Write("received something");
			PlaymoreClient.Gui.MainForm.LOGGER.Info(string.Format("Recv {0} bytes", len));
			this.receivebuffer.Append(buffer, idx, len);
			List<object> objs = RtmpProtocolDecoder.DecodeBuffer(this.remotecontext, this.receivebuffer);
			if (objs != null)
			{
				foreach (object obj in objs)
				{
					RtmpPacket rtmpPacket = obj as RtmpPacket;
					if (rtmpPacket != null)
					{
						Notify message = rtmpPacket.Message as Notify;
						if (message == null)
						{
							PlaymoreClient.Gui.MainForm.LOGGER.Info(string.Format("Recv {0} (Id:{1})", rtmpPacket.Message, rtmpPacket.Header.ChannelId));
						}
						else
						{
							Notify item = null;
							if (!RtmpUtil.IsResult(message))
							{
								this.OnNotify(message);
							}
							else
							{
								lock (this.InvokeList)
								{
									int num = this.InvokeList.FindIndex((Notify i) => i.InvokeId == message.InvokeId);
									if (num != -1)
									{
										item = this.InvokeList[num];
										this.InvokeList.RemoveAt(num);
									}
								}
								if (item != null)
								{
									this.OnCall(item, message);
                                  //  PlaymoreClient.Gui.MainForm.LOGGER.Info((object)string.Format("Ret  ({0}) (Id:{1})", (object)string.Join(", ", IEnumerable.Select<object, string>((IEnumerable<object>)item.ServiceCall.Arguments, (Func<object, string>)(o => o.ToString()))), (object)rtmpPacket.Header.ChannelId));
								}
							}
						}
					}
					else if (!(obj is ByteBuffer))
					{
						PlaymoreClient.Gui.MainForm.LOGGER.Warn(string.Format("Unknown object {0}", obj.GetType()));
					}
					if (obj == null)
					{
						continue;
					}
					if (rtmpPacket == null || !(rtmpPacket.Message is Notify))
					{
						ByteBuffer byteBuffer = RtmpProtocolEncoder.Encode(this.remotecontext, obj);
						if (byteBuffer != null)
						{
							byte[] array = byteBuffer.ToArray();
							base.OnReceive(array, 0, (int)array.Length);
						}
						else
						{
							PlaymoreClient.Gui.MainForm.LOGGER.Fatal(string.Concat("Unable to encode ", obj));
						}
					}
					else
					{
						this.InternalReceive((Notify)rtmpPacket.Message);
						this.remotecontext.ObjectEncoding = ObjectEncoding.AMF3;
					}
				}
			}
		}

		protected override void OnSend(byte[] buffer, int idx, int len)
		{
			if (this.postbuffer != null)
			{
				this.postbuffer.Append(buffer, idx, len);
				if (this.postbuffer.Length > (long)4)
				{
					int num = this.postbuffer.GetInt();
					this.postbuffer.Dispose();
					this.postbuffer = null;
					if (num == 1347375956)
					{
						PlaymoreClient.Gui.MainForm.LOGGER.Info(string.Format("Rejecting POST request", len));
						this.Stop();
						return;
					}
				}
			}
			PlaymoreClient.Gui.MainForm.LOGGER.Info(string.Format("Send {0} bytes", len));
			this.sendbuffer.Append(buffer, idx, len);
			List<object> objs = RtmpProtocolDecoder.DecodeBuffer(this.sourcecontext, this.sendbuffer);
			if (objs != null)
			{
				foreach (object obj in objs)
				{
					RtmpPacket rtmpPacket = obj as RtmpPacket;
					if (rtmpPacket != null)
					{
						Notify message = rtmpPacket.Message as Notify;
						if (message == null)
						{
							PlaymoreClient.Gui.MainForm.LOGGER.Info(string.Format("Sent {0} (Id:{1})", rtmpPacket.Message.GetType(), rtmpPacket.Header.ChannelId));
						}
						else
						{
							lock (this.InvokeList)
							{
								this.InvokeList.Add(message);
							}
							//PlaymoreClient.Gui.MainForm.LOGGER.Info((object) string.Format("Call {0}({1}) (Id:{2})", (object) message.ServiceCall.ServiceMethodName, (object) string.Join(", ", Enumerable.Select<object, string>((IEnumerable<object>) message.ServiceCall.Arguments, (Func<object, string>) (o => o.ToString()))), (object) rtmpPacket.Header.ChannelId));
						}
					}
					else if (!(obj is ByteBuffer))
					{
						PlaymoreClient.Gui.MainForm.LOGGER.Warn(string.Format("Unknown object {0}", obj.GetType()));
					}
					if (obj == null)
					{
						continue;
					}
					if (rtmpPacket == null || !(rtmpPacket.Message is Notify))
					{
						ByteBuffer byteBuffer = RtmpProtocolEncoder.Encode(this.sourcecontext, obj);
						if (byteBuffer != null)
						{
							byte[] array = byteBuffer.ToArray();
							base.OnSend(array, 0, (int)array.Length);
						}
						else
						{
							PlaymoreClient.Gui.MainForm.LOGGER.Fatal(string.Concat("Unable to encode ", obj));
						}
					}
					else
					{
						this.InternalSend((Notify)rtmpPacket.Message, true);
						this.sourcecontext.ObjectEncoding = ObjectEncoding.AMF3;
					}
				}
			}
		}

		public void Send(Notify notify)
		{
			lock (this.WaitLock)
			{
				if (this.WaitInvokeList != null)
				{
					this.WaitInvokeList.Add(new CallResultWait(notify, false));
				}
				else
				{
					return;
				}
			}
			notify.InvokeId = this.CurrentInvoke.Increment();
			this.InternalSend(notify, false);
		}
	}
}