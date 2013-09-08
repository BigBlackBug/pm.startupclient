using FluorineFx.Messaging.Rtmp.Event;
using System;

namespace PlaymoreClient.Messaging
{
	public delegate void CallHandler(object sender, Notify call, Notify result);
}