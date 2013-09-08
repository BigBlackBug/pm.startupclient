using PlaymoreClient.Flash;
using PlaymoreClient.Messages;
using PlaymoreClient.Messages.GameStats.PlayerStats;
using FluorineFx;
using NotMissing;
using System;
using System.Runtime.CompilerServices;

namespace PlaymoreClient.Messages.GameStats
{
	[Message(".EndOfGameStats")]
	public class EndOfGameStats : MessageObject, ICloneable
	{
		[InternalName("basePoints")]
		public int BasePoints
		{
			get;
			set;
		}

		[InternalName("boostIpEarned")]
		public int BoostIpEarned
		{
			get;
			set;
		}

		[InternalName("boostXpEarned")]
		public int BoostXpEarned
		{
			get;
			set;
		}

		[InternalName("completionBonusPoints")]
		public int CompletionBonusPoints
		{
			get;
			set;
		}

		[InternalName("difficulty")]
		public string Difficulty
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

		[InternalName("experienceEarned")]
		public int ExperienceEarned
		{
			get;
			set;
		}

		[InternalName("experienceTotal")]
		public int ExperienceTotal
		{
			get;
			set;
		}

		public int ExpPointsToNextLevel
		{
			get;
			set;
		}

		[InternalName("firstWinBonus")]
		public int FirstWinBonus
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

		[InternalName("gameLength")]
		public int GameLength
		{
			get;
			set;
		}

		[InternalName("gameMode")]
		public string GameMode
		{
			get;
			set;
		}

		[InternalName("gameType")]
		public string GameType
		{
			get;
			set;
		}

		[InternalName("imbalancedTeamsNoPoints")]
		public bool ImbalancedTeamsNoPoints
		{
			get;
			set;
		}

		[InternalName("ipEarned")]
		public int IpEarned
		{
			get;
			set;
		}

		[InternalName("ipTotal")]
		public int IpTotal
		{
			get;
			set;
		}

		[InternalName("leveledUp")]
		public bool LeveledUp
		{
			get;
			set;
		}

		public int LocationBoostIpEarned
		{
			get;
			set;
		}

		public int LocationBoostXpEarned
		{
			get;
			set;
		}

		[InternalName("loyaltyBoostIpEarned")]
		public int LoyaltyBoostIpEarned
		{
			get;
			set;
		}

		[InternalName("loyaltyBoostXpEarned")]
		public int LoyaltyBoostXpEarned
		{
			get;
			set;
		}

		[InternalName("odinBonusIp")]
		public int OdinBonusIp
		{
			get;
			set;
		}

		[InternalName("otherTeamPlayerParticipantStats")]
		public PlayerStatsSummaryList OtherTeamPlayerStats
		{
			get;
			set;
		}

		[InternalName("queueBonusEarned")]
		public int QueueBonusEarned
		{
			get;
			set;
		}

		[InternalName("queueType")]
		public string QueueType
		{
			get;
			set;
		}

		[InternalName("ranked")]
		public bool Ranked
		{
			get;
			set;
		}

		[InternalName("skinIndex")]
		public int SkinIndex
		{
			get;
			set;
		}

		[InternalName("talentPointsGained")]
		public int TalentPointsGained
		{
			get;
			set;
		}

		[InternalName("teamPlayerParticipantStats")]
		public PlayerStatsSummaryList TeamPlayerStats
		{
			get;
			set;
		}

		[InternalName("timeUntilNextFirstWinBonus")]
		public int TimeUntilNextFirstWinBonus
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

		public EndOfGameStats() : base(null)
		{
		}

		public EndOfGameStats(ASObject obj) : base(obj)
		{
            base.ObjectType = "EndOfGameStats";
			BaseObject.SetFields<EndOfGameStats>(this, obj);
		}

		public object Clone()
		{
			EndOfGameStats endOfGameStat = new EndOfGameStats();
			endOfGameStat.BasePoints = this.BasePoints;
			endOfGameStat.BoostIpEarned = this.BoostIpEarned;
			endOfGameStat.BoostXpEarned = this.BoostXpEarned;
			endOfGameStat.CompletionBonusPoints = this.CompletionBonusPoints;
			endOfGameStat.Difficulty = this.Difficulty;
			endOfGameStat.Elo = this.Elo;
			endOfGameStat.EloChange = this.EloChange;
			endOfGameStat.ExperienceEarned = this.ExperienceEarned;
			endOfGameStat.ExperienceTotal = this.ExperienceTotal;
			endOfGameStat.ExpPointsToNextLevel = this.ExpPointsToNextLevel;
			endOfGameStat.FirstWinBonus = this.FirstWinBonus;
			endOfGameStat.GameId = this.GameId;
			endOfGameStat.GameLength = this.GameLength;
			endOfGameStat.GameMode = this.GameMode;
			endOfGameStat.GameType = this.GameType;
			endOfGameStat.ImbalancedTeamsNoPoints = this.ImbalancedTeamsNoPoints;
			endOfGameStat.IpEarned = this.IpEarned;
			endOfGameStat.IpTotal = this.IpTotal;
			endOfGameStat.LeveledUp = this.LeveledUp;
			endOfGameStat.LocationBoostIpEarned = this.LocationBoostIpEarned;
			endOfGameStat.LocationBoostXpEarned = this.LocationBoostXpEarned;
			endOfGameStat.LoyaltyBoostIpEarned = this.LoyaltyBoostIpEarned;
			endOfGameStat.LoyaltyBoostXpEarned = this.LoyaltyBoostXpEarned;
			endOfGameStat.OdinBonusIp = this.OdinBonusIp;
			endOfGameStat.OtherTeamPlayerStats = new PlayerStatsSummaryList(this.OtherTeamPlayerStats.Clone<PlayerStatsSummary>());
			endOfGameStat.QueueBonusEarned = this.QueueBonusEarned;
			endOfGameStat.QueueType = this.QueueType;
			endOfGameStat.Ranked = this.Ranked;
			endOfGameStat.SkinIndex = this.SkinIndex;
			endOfGameStat.TalentPointsGained = this.TalentPointsGained;
			endOfGameStat.TeamPlayerStats = new PlayerStatsSummaryList(this.TeamPlayerStats.Clone<PlayerStatsSummary>());
			endOfGameStat.TimeUntilNextFirstWinBonus = this.TimeUntilNextFirstWinBonus;
			endOfGameStat.UserId = this.UserId;
			endOfGameStat.TimeStamp = base.TimeStamp;
			return endOfGameStat;
		}
	}
}