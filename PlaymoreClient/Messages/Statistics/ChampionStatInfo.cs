using PlaymoreClient.Flash;
using FluorineFx;
using System;
using System.Runtime.CompilerServices;

namespace PlaymoreClient.Messages.Statistics
{
	public class ChampionStatInfo : BaseObject
	{
		[InternalName("championId")]
		public int ChampionId
		{
			get;
			set;
		}

		[InternalName("stats")]
		public AggregatedStatList Stats
		{
			get;
			set;
		}

		[InternalName("totalGamesPlayed")]
		public int TotalGamesPlayed
		{
			get;
			set;
		}

		public ChampionStatInfo(ASObject obj) : base(obj)
		{
			this.Stats = new AggregatedStatList();
			BaseObject.SetFields<ChampionStatInfo>(this, obj);
		}
	}
}