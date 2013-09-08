using FluorineFx;
using FluorineFx.AMF3;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PlaymoreClient.Messages.GameStats.PlayerStats
{
	public class PlayerStatsSummaryList : List<PlayerStatsSummary>
	{
		protected readonly ArrayCollection Base;

		public PlayerStatsSummaryList()
		{
		}

		public PlayerStatsSummaryList(IEnumerable<PlayerStatsSummary> collection) : base(collection)
		{
		}

		public PlayerStatsSummaryList(ArrayCollection thebase)
		{
			this.Base = thebase;
			if (this.Base == null)
			{
				return;
			}
			foreach (object @base in this.Base)
			{
				base.Add(new PlayerStatsSummary(@base as ASObject));
			}
		}
	}
}