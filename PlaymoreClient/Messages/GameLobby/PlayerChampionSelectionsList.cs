using FluorineFx;
using FluorineFx.AMF3;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PlaymoreClient.Messages.GameLobby
{
	public class PlayerChampionSelectionsList : List<PlayerChampionSelection>
	{
		protected readonly ArrayCollection Base;

		public PlayerChampionSelectionsList()
		{
		}

		public PlayerChampionSelectionsList(IEnumerable<PlayerChampionSelection> collection) : base(collection)
		{
		}

		public PlayerChampionSelectionsList(ArrayCollection thebase)
		{
			this.Base = thebase;
			if (this.Base == null)
			{
				return;
			}
			foreach (object @base in this.Base)
			{
				base.Add(new PlayerChampionSelection(@base as ASObject));
			}
		}
        public override string ToString()
        {
            string str = "";
            foreach (object p in base.ToArray())
            {
                str += "(" + p.ToString() + ")";
            }
            return "{" + str + "}";
        }
     
	}
}