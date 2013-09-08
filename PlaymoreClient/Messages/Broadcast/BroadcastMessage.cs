using System;
using System.Runtime.CompilerServices;

namespace PlaymoreClient.Messages.Broadcast
{
	public class BroadcastMessage
	{
		public bool active
		{
			get;
			set;
		}

		public string content
		{
			get;
			set;
		}

		public long id
		{
			get;
			set;
		}

		public string messageKey
		{
			get;
			set;
		}

		public string severity
		{
			get;
			set;
		}

		public BroadcastMessage()
		{
		}
	}
}