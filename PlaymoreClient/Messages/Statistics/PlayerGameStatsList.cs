using PlaymoreClient.Flash;
using FluorineFx;
using FluorineFx.AMF3;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PlaymoreClient.Messages.Statistics
{
	public class PlayerGameStatsList : BaseList<PlayerGameStats>
	{
		public PlayerGameStatsList() : base(null)
		{
		}

		public PlayerGameStatsList(ArrayCollection obj) : base(obj)
		{
			if (obj == null)
			{
				return;
			}
			foreach (object obj1 in obj)
			{
				base.Add(new PlayerGameStats(obj1 as ASObject));
			}
		}
	}
}