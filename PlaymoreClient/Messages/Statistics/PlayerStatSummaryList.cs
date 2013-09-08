using PlaymoreClient.Flash;
using FluorineFx;
using FluorineFx.AMF3;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PlaymoreClient.Messages.Statistics
{
	public class PlayerStatSummaryList : BaseList<PlayerStatSummary>
	{
		public PlayerStatSummaryList() : base(null)
		{
		}

		public PlayerStatSummaryList(ArrayCollection obj) : base(obj)
		{
			if (obj == null)
			{
				return;
			}
			foreach (object obj1 in obj)
			{
				base.Add(new PlayerStatSummary(obj1 as ASObject));
			}
		}
	}
}