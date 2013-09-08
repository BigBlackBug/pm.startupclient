using PlaymoreClient.Flash;
using PlaymoreClient.Messages;
using FluorineFx;
using System;
using System.Runtime.CompilerServices;

namespace PlaymoreClient.Messages.Statistics
{
	[Message(".PlayerLifetimeStats")]
	public class PlayerLifetimeStats : MessageObject
	{
		[InternalName("dataVersion")]
		public int DataVersion
		{
			get;
			set;
		}

		[InternalName("dodgeStreak")]
		public int DodgeStreak
		{
			get;
			set;
		}

		[InternalName("leaverPenaltyStats")]
		public ASObject LeaverPenaltyStats
		{
			get;
			set;
		}

		[InternalName("playerStats")]
		public ASObject PlayerStats
		{
			get;
			set;
		}

		[InternalName("playerStatSummaries")]
		public PlaymoreClient.Messages.Statistics.PlayerStatSummaries PlayerStatSummaries
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

		public PlayerLifetimeStats(ASObject obj) : base(obj)
		{
			this.PlayerStatSummaries = new PlaymoreClient.Messages.Statistics.PlayerStatSummaries();
			BaseObject.SetFields<PlayerLifetimeStats>(this, obj);
		}
	}
}