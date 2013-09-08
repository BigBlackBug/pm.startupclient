using PlaymoreClient.Flash;
using FluorineFx;
using FluorineFx.AMF3;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PlaymoreClient.Messages.Statistics
{
	public class FellowPlayerInfoList : BaseList<FellowPlayerInfo>
	{
		public FellowPlayerInfoList() : base(null)
		{
		}

		public FellowPlayerInfoList(ArrayCollection obj) : base(obj)
		{
			if (obj == null)
			{
				return;
			}
			foreach (object obj1 in obj)
			{
				base.Add(new FellowPlayerInfo(obj1 as ASObject));
			}
		}
	}
}