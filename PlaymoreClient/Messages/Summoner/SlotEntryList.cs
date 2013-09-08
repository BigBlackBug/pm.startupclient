using PlaymoreClient.Flash;
using FluorineFx;
using FluorineFx.AMF3;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PlaymoreClient.Messages.Summoner
{
	public class SlotEntryList : BaseList<SlotEntry>
	{
		public SlotEntryList() : base(null)
		{
		}

		public SlotEntryList(ArrayCollection obj) : base(obj)
		{
			if (obj == null)
			{
				return;
			}
			foreach (object obj1 in obj)
			{
				base.Add(new SlotEntry(obj1 as ASObject));
			}
		}
	}
}