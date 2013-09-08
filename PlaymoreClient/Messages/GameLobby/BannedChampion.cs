using PlaymoreClient.Flash;
using FluorineFx;
using System;
using System.Runtime.CompilerServices;

namespace PlaymoreClient.Messages.GameLobby
{
	public class BannedChampion : BaseObject, ICloneable
	{
		[InternalName("championId")]
		public int ChampionId
		{
			get;
			set;
		}

		[InternalName("dataVersion")]
		public object DataVersion
		{
			get;
			set;
		}

		[InternalName("futureData")]
		public object FutureData
		{
			get;
			set;
		}

		[InternalName("pickTurn")]
		public int PickTurn
		{
			get;
			set;
		}

		[InternalName("teamId")]
		public int TeamId
		{
			get;
			set;
		}

		public BannedChampion() : base(null)
		{
		}

		public BannedChampion(ASObject body) : base(body)
		{
			BaseObject.SetFields<BannedChampion>(this, body);
		}

		public object Clone()
		{
			return new BannedChampion();
		}
	}
}