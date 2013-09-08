using PlaymoreClient.Flash;
using FluorineFx;
using FluorineFx.AMF3;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PlaymoreClient.Messages.Statistics
{
	public class ChampionStatInfoList : BaseList<ChampionStatInfo>
	{
		public ChampionStatInfoList() : base(null)
		{
		}

		public ChampionStatInfoList(ArrayCollection obj) : base(obj)
		{
			if (obj == null)
			{
				return;
			}
			foreach (object obj1 in obj)
			{
				base.Add(new ChampionStatInfo(obj1 as ASObject));
			}
		}
	}
}