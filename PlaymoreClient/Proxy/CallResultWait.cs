using FluorineFx.Messaging.Rtmp.Event;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace PlaymoreClient.Proxy
{
	public class CallResultWait
	{
		public bool Blocking
		{
			get;
			set;
		}

		public Notify Call
		{
			get;
			set;
		}

		public Notify Result
		{
			get;
			set;
		}

		public AutoResetEvent Wait
		{
			get;
			set;
		}

		public CallResultWait(Notify call, bool blocking)
		{
			this.Blocking = blocking;
			this.Call = call;
			this.Wait = new AutoResetEvent(false);
		}
	}
}