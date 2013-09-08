using PlaymoreClient.Flash;
using PlaymoreClient.Messages;
using FluorineFx;
using System;
using System.Runtime.CompilerServices;

namespace PlaymoreClient.Messages.Champion
{
	[Message(".ChampionSkinDTO")]
	public class ChampionSkinDTO : BaseObject
	{
		[InternalName("championId")]
		public int ChampionId
		{
			get;
			set;
		}

		[InternalName("endDate")]
		public long EndDate
		{
			get;
			set;
		}

		[InternalName("freeToPlayReward")]
		public bool FreeToPlayReward
		{
			get;
			set;
		}

		[InternalName("lastSelected")]
		public bool LastSelected
		{
			get;
			set;
		}

		[InternalName("owned")]
		public bool Owned
		{
			get;
			set;
		}

		[InternalName("purchaseDate")]
		public long PurchaseDate
		{
			get;
			set;
		}

		[InternalName("skinId")]
		public int SkinId
		{
			get;
			set;
		}

		[InternalName("stillObtainable")]
		public bool StillObtainable
		{
			get;
			set;
		}

		[InternalName("winCountRemaining")]
		public int WinCountRemaining
		{
			get;
			set;
		}

		public ChampionSkinDTO(ASObject obj) : base(obj)
		{
			BaseObject.SetFields<ChampionSkinDTO>(this, obj);
		}
	}
}