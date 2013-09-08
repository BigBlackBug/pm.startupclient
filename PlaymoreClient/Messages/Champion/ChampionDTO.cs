using PlaymoreClient.Flash;
using PlaymoreClient.Messages;
using FluorineFx;
using System;
using System.Runtime.CompilerServices;

namespace PlaymoreClient.Messages.Champion
{
	[Message(".ChampionDTO")]
	public class ChampionDTO : BaseObject
	{
		[InternalName("active")]
		public bool Active
		{
			get;
			set;
		}

		[InternalName("botEnabled")]
		public bool BotEnabled
		{
			get;
			set;
		}

		[InternalName("championId")]
		public int ChampionId
		{
			get;
			set;
		}

		[InternalName("championSkins")]
		public ChampionSkinDTOList ChampionSkins
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

		[InternalName("freeToPlay")]
		public bool FreeToPlay
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

		[InternalName("owned")]
		public bool Owned
		{
			get;
			set;
		}

		[InternalName("purchased")]
		public long Purchased
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

		[InternalName("rankedPlayEnabled")]
		public bool RankedPlayEnabled
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

		public ChampionDTO(ASObject obj) : base(obj)
		{
			this.ChampionSkins = new ChampionSkinDTOList();
			BaseObject.SetFields<ChampionDTO>(this, obj);
		}
	}
}