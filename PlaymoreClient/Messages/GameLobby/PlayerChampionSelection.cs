using PlaymoreClient.Flash;
using FluorineFx;
using System;
using System.Runtime.CompilerServices;

namespace PlaymoreClient.Messages.GameLobby
{
	public class PlayerChampionSelection : BaseObject, ICloneable
    {
        public override string ToString()
        {
            return "champ_id " + ChampionId + " summoner internal name " + SummonerInternalName;
        }
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

		[InternalName("selectedSkinIndex")]
		public int SelectedSkinIndex
		{
			get;
			set;
		}

		[InternalName("spell1Id")]
		public int Spell1Id
		{
			get;
			set;
		}

		[InternalName("spell2Id")]
		public int Spell2Id
		{
			get;
			set;
		}

		[InternalName("summonerInternalName")]
		public string SummonerInternalName
		{
			get;
			set;
		}

		public PlayerChampionSelection() : base(null)
		{
		}

		public PlayerChampionSelection(ASObject body) : base(body)
		{
			BaseObject.SetFields<PlayerChampionSelection>(this, body);
		}

		public object Clone()
		{
			return new PlayerChampionSelection();
		}
	}
}