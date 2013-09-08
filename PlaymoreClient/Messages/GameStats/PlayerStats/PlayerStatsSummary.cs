using PlaymoreClient.Flash;
using PlaymoreClient.Messages.GameStats;
using FluorineFx;
using NotMissing;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace PlaymoreClient.Messages.GameStats.PlayerStats
{
	[DebuggerDisplay("{SummonerName}")]
	public class PlayerStatsSummary : BaseObject, ICloneable
	{
		[InternalName("botPlayer")]
		public bool BotPlayer
		{
			get;
			set;
		}

		[InternalName("elo")]
		public int Elo
		{
			get;
			set;
		}

		[InternalName("eloChange")]
		public int EloChange
		{
			get;
			set;
		}

		[InternalName("gameId")]
		public long GameId
		{
			get;
			set;
		}

		public GameItems Items
		{
			get;
			set;
		}

		[InternalName("leaver")]
		public bool Leaver
		{
			get;
			set;
		}

		[InternalName("leaves")]
		public int Leaves
		{
			get;
			set;
		}

		[InternalName("level")]
		public int Level
		{
			get;
			set;
		}

		[InternalName("losses")]
		public int Losses
		{
			get;
			set;
		}

		[InternalName("profileIconId")]
		public int ProfileIconId
		{
			get;
			set;
		}

		[InternalName("skinName")]
		public string SkinName
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

		[InternalName("statistics")]
		public PlayerStatList Statistics
		{
			get;
			set;
		}

		[InternalName("summonerName")]
		public string SummonerName
		{
			get;
			set;
		}

		[InternalName("teamId")]
		public long TeamId
		{
			get;
			set;
		}

		[InternalName("userId")]
		public long UserId
		{
			get;
			set;
		}

		[InternalName("wins")]
		public int Wins
		{
			get;
			set;
		}

		public PlayerStatsSummary() : base(null)
		{
		}

		public PlayerStatsSummary(ASObject body) : base(body)
		{
			BaseObject.SetFields<PlayerStatsSummary>(this, body);
		}

		public object Clone()
		{
			PlayerStatsSummary playerStatsSummary = new PlayerStatsSummary();
			playerStatsSummary.BotPlayer = this.BotPlayer;
			playerStatsSummary.Elo = this.Elo;
			playerStatsSummary.EloChange = this.EloChange;
			playerStatsSummary.GameId = this.GameId;
			playerStatsSummary.Leaver = this.Leaver;
			playerStatsSummary.Leaves = this.Leaves;
			playerStatsSummary.Level = this.Level;
			playerStatsSummary.Losses = this.Losses;
			playerStatsSummary.ProfileIconId = this.ProfileIconId;
			playerStatsSummary.SkinName = this.SkinName;
			playerStatsSummary.Spell1Id = this.Spell1Id;
			playerStatsSummary.Spell2Id = this.Spell2Id;
			playerStatsSummary.Statistics = new PlayerStatList(this.Statistics.Clone<PlayerStat>());
			playerStatsSummary.SummonerName = this.SummonerName;
			playerStatsSummary.TeamId = this.TeamId;
			playerStatsSummary.UserId = this.UserId;
			playerStatsSummary.Wins = this.Wins;
			return playerStatsSummary;
		}
	}
}