using PlaymoreClient.Flash;
using FluorineFx;
using System;
using System.Runtime.CompilerServices;

namespace PlaymoreClient.Messages.Statistics
{
	public class PlayerGameStats : BaseObject
	{
		[InternalName("adjustedRating")]
		public int AdjustedRating
		{
			get;
			set;
		}

		[InternalName("afk")]
		public bool Afk
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

		[InternalName("championId")]
		public int ChampionId
		{
			get;
			set;
		}

		[InternalName("difficulty")]
		public object Difficulty
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

		[InternalName("fellowPlayers")]
		public FellowPlayerInfoList FellowPlayers
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

		[InternalName("gameMapId")]
		public int GameMapId
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

		[InternalName("gameTypeEnum")]
		public string GameTypeEnum
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

		[InternalName("KCoefficient")]
		public int KCoefficient
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

		[InternalName("predictedWinPct")]
		public double PredictedWinPct
		{
			get;
			set;
		}

		[InternalName("premadeSize")]
		public int PremadeSize
		{
			get;
			set;
		}

		[InternalName("premadeTeam")]
		public int PremadeTeam
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

		[InternalName("rating")]
		public int Rating
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

		[InternalName("skinName")]
		public string SkinName
		{
			get;
			set;
		}

		[InternalName("spell1")]
		public int Spell1
		{
			get;
			set;
		}

		[InternalName("spell2")]
		public int Spell2
		{
			get;
			set;
		}

		[InternalName("statistics")]
		public RawStatList Statistics
		{
			get;
			set;
		}

		[InternalName("subType")]
		public string SubType
		{
			get;
			set;
		}

		[InternalName("summonerId")]
		public long SummonerId
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

		[InternalName("teamRating")]
		public int TeamRating
		{
			get;
			set;
		}

		[InternalName("timeInQueue")]
		public int TimeInQueue
		{
			get;
			set;
		}

		public PlayerGameStats(ASObject obj) : base(obj)
		{
			BaseObject.SetFields<PlayerGameStats>(this, obj);
		}
	}
}