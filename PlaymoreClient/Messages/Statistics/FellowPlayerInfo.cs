using PlaymoreClient.Flash;
using FluorineFx;
using System;
using System.Runtime.CompilerServices;

namespace PlaymoreClient.Messages.Statistics
{
	public class FellowPlayerInfo : BaseObject
	{
		[InternalName("championId")]
		public int ChampionId
		{
			get;
			set;
		}

		[InternalName("summonerId")]
		public long SummonerId
		{
			get;
			set;
		}

		[InternalName("teamId")]
		public int TeamId
		{
			get;
			set;
		}

		public FellowPlayerInfo(ASObject obj) : base(obj)
		{
			BaseObject.SetFields<FellowPlayerInfo>(this, obj);
		}
	}
}