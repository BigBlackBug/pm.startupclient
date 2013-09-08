using PlaymoreClient.Flash;
using FluorineFx;
using System;
using System.Runtime.CompilerServices;

namespace PlaymoreClient.Messages.Statistics
{
	public class PlayerStatSummaries : BaseObject
	{
		[InternalName("playerStatSummarySet")]
		public PlayerStatSummaryList PlayerStatSummarySet
		{
			get;
			set;
		}

		[InternalName("userId")]
		public int UserId
		{
			get;
			set;
		}

		public PlayerStatSummaries() : base(null)
		{
		}

		public PlayerStatSummaries(ASObject obj) : base(obj)
		{
			this.PlayerStatSummarySet = new PlayerStatSummaryList();
			BaseObject.SetFields<PlayerStatSummaries>(this, obj);
		}
	}
}