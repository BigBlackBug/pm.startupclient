using PlaymoreClient.Flash;
using FluorineFx;
using FluorineFx.AMF3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PlaymoreClient.Messages.Statistics
{
	public class RawStatList : BaseList<RawStat>
	{
		public RawStatList() : base(null)
		{
		}

		public RawStatList(ArrayCollection obj) : base(obj)
		{
			if (obj == null)
			{
				return;
			}
			foreach (object obj1 in obj)
			{
				base.Add(new RawStat(obj1 as ASObject));
			}
		}

		public object Get(string key)
		{
			RawStat rawStat = base.Find((RawStat r) => r.StatType == key);
			if (rawStat == null)
			{
				return null;
			}
			return rawStat.Value;
		}

		public int GetInt(string key)
		{
			return Convert.ToInt32(this.Get(key));
		}
	}
}