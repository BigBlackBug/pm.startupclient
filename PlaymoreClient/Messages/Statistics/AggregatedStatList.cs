using PlaymoreClient.Flash;
using FluorineFx;
using FluorineFx.AMF3;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PlaymoreClient.Messages.Statistics
{
	public class AggregatedStatList : BaseList<AggregatedStat>
	{
		public AggregatedStatList() : base(null)
		{
		}

		public AggregatedStatList(ArrayCollection obj) : base(obj)
		{
			if (obj == null)
			{
				return;
			}
			foreach (object obj1 in obj)
			{
				base.Add(new AggregatedStat(obj1 as ASObject));
			}
		}
	}
}