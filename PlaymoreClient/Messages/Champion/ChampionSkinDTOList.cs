using PlaymoreClient.Flash;
using FluorineFx;
using FluorineFx.AMF3;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PlaymoreClient.Messages.Champion
{
	public class ChampionSkinDTOList : BaseList<ChampionSkinDTO>
	{
		public ChampionSkinDTOList() : base(null)
		{
		}

		public ChampionSkinDTOList(ArrayCollection obj) : base(obj)
		{
			if (obj == null)
			{
				return;
			}
			foreach (object obj1 in obj)
			{
				base.Add(new ChampionSkinDTO(obj1 as ASObject));
			}
		}
	}
}