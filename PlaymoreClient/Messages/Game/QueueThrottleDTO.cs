using System;
using System.Runtime.CompilerServices;

namespace PlaymoreClient.Messages.Game
{
	public class QueueThrottleDTO
	{
		public bool generic
		{
			get;
			set;
		}

		public int level
		{
			get;
			set;
		}

		public string mode
		{
			get;
			set;
		}

		public int queueId
		{
			get;
			set;
		}

		public QueueThrottleDTO()
		{
		}
	}
}