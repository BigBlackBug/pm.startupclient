using FluorineFx;
using FluorineFx.AMF3;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PlaymoreClient.Messages.GameStats.PlayerStats
{
	public class PlayerStatList : List<PlayerStat>
	{
		protected readonly ArrayCollection Base;

		public PlayerStatList()
		{
		}

		public PlayerStatList(IEnumerable<PlayerStat> collection) : base(collection)
		{
		}

		public PlayerStatList(ArrayCollection thebase)
		{
			this.Base = thebase;
			if (this.Base == null)
			{
				return;
			}
			foreach (object @base in this.Base)
			{
				base.Add(new PlayerStat(@base as ASObject));
			}
		}
	}
}