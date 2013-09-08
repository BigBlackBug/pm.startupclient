using System;

namespace PlaymoreClient.Messaging
{
	public delegate void ProcessObjectHandler(object sender, object obj, long timestamp);
}