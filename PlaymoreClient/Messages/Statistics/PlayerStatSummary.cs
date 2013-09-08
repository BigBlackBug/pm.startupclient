using PlaymoreClient.Flash;
using FluorineFx;
using System;
using System.Runtime.CompilerServices;

namespace PlaymoreClient.Messages.Statistics
{
	public class PlayerStatSummary : BaseObject
	{
		[InternalName("aggregatedStats")]
		public ASObject AggregatedStats
		{
			get;
			set;
		}

		[InternalName("leaves")]
		public int Leaves
		{
			get;
			set;
		}

		[InternalName("losses")]
		public int Losses
		{
			get;
			set;
		}

		[InternalName("maxRating")]
		public int MaxRating
		{
			get;
			set;
		}

		[InternalName("playerStatSummaryType")]
		public string PlayerStatSummaryType
		{
			get;
			set;
		}

		[InternalName("playerStatSummaryTypeString")]
		public string PlayerStatSummaryTypeString
		{
			get;
			set;
		}

		[InternalName("rating")]
		public int Rating
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

		[InternalName("wins")]
		public int Wins
		{
			get;
			set;
		}

		public PlayerStatSummary(ASObject obj) : base(obj)
		{
			BaseObject.SetFields<PlayerStatSummary>(this, obj);
		}
	}
}