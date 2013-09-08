using PlaymoreClient.Assets;
using PlaymoreClient.Flash;
using FluorineFx;
using System;
using System.Runtime.CompilerServices;

namespace PlaymoreClient.Messages.Statistics
{
	public class AggregatedStat : BaseObject
	{
		[InternalName("statType")]
		public string StatType
		{
			get;
			set;
		}

		public string StatTypeString
		{
			get
			{
				return StatsData.Get(this.StatType);
			}
		}

		[InternalName("value")]
		public object Value
		{
			get;
			set;
		}

		public AggregatedStat(ASObject obj) : base(obj)
		{
			BaseObject.SetFields<AggregatedStat>(this, obj);
		}
	}
}