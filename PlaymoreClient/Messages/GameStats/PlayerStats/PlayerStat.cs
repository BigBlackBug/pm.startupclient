using PlaymoreClient.Flash;
using FluorineFx;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace PlaymoreClient.Messages.GameStats.PlayerStats
{
	[DebuggerDisplay("{DisplayName}")]
	public class PlayerStat : BaseObject, ICloneable
	{
		[InternalName("statTypeName")]
		public string StatTypeName
		{
			get;
			set;
		}

		[InternalName("value")]
		public int Value
		{
			get;
			set;
		}

		public PlayerStat() : base(null)
		{
		}

		public PlayerStat(ASObject thebase) : base(thebase)
		{
			BaseObject.SetFields<PlayerStat>(this, this.Base);
		}

		public object Clone()
		{
			PlayerStat playerStat = new PlayerStat();
			playerStat.StatTypeName = this.StatTypeName;
			playerStat.Value = this.Value;
			return playerStat;
		}
	}
}