using FluorineFx;
using FluorineFx.AMF3;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PlaymoreClient.Messages.GameLobby
{
	public class BannedChampionList : List<BannedChampion>
	{
		protected readonly ArrayCollection Base;

		public BannedChampionList()
		{
		}

		public BannedChampionList(IEnumerable<BannedChampion> collection) : base(collection)
		{
		}

		public BannedChampionList(ArrayCollection thebase)
		{
			this.Base = thebase;
			if (this.Base == null)
			{
				return;
			}
			foreach (object @base in this.Base)
			{
				base.Add(new BannedChampion(@base as ASObject));
			}
		}
	}
}