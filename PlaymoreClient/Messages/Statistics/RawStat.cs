using PlaymoreClient.Flash;
using FluorineFx;
using System;
using System.Runtime.CompilerServices;

namespace PlaymoreClient.Messages.Statistics
{
	public class RawStat : BaseObject
	{
		public const string PHYSICAL_DAMAGE_DEALT_PLAYER = "PHYSICAL_DAMAGE_DEALT_PLAYER";

		public const string ITEM4 = "ITEM3";

		public const string NODE_NEUTRALIZE_ASSIST = "NODE_NEUTRALIZE_ASSIST";

		public const string TIME_PLAYED = "TIME_PLAYED";

		public const string COMBAT_PLAYER_SCORE = "COMBAT_PLAYER_SCORE";

		public const string ITEM5 = "ITEM4";

		public const string NODE_CAPTURE_ASSIST = "NODE_CAPTURE_ASSIST";

		public const string ITEM3 = "ITEM2";

		public const string ASSISTS = "ASSISTS";

		public const string TOTAL_DAMAGE_TAKEN = "TOTAL_DAMAGE_TAKEN";

		public const string LOSE = "LOSE";

		public const string WINS = "WIN";

		public const string LEAVES = "LEAVES";

		public const string ITEM2 = "ITEM1";

		public const string TOTAL_HEAL = "TOTAL_HEAL";

		public const string LARGEST_MULTI_KILL = "LARGEST_MULTI_KILL";

		public const string OFFENSE_PLAYER_SCORE = "OFFENSE_PLAYER_SCORE";

		public const string ITEM1 = "ITEM0";

		public const string NODE_NEUTRALIZE = "NODE_NEUTRALIZE";

		public const string ITEM6 = "ITEM5";

		public const string BARRACKS_KILLED = "BARRACKS_KILLED";

		public const string SPELL2_CAST = "SPELL2_CAST";

		public const string MAGIC_DAMAGE_DEALT_PLAYER = "MAGIC_DAMAGE_DEALT_PLAYER";

		public const string TEAM_OBJECTIVE = "TEAM_OBJECTIVE";

		public const string WIN = "WIN";

		public const string GOLD_EARNED = "GOLD_EARNED";

		public const string PHYSICAL_DAMAGE_TAKEN = "PHYSICAL_DAMAGE_TAKEN";

		public const string SPELL1_CAST = "SPELL1_CAST";

		public const string MAGIC_DAMAGE_TAKEN = "MAGIC_DAMAGE_TAKEN";

		public const string TOTAL_TIME_SPENT_DEAD = "TOTAL_TIME_SPENT_DEAD";

		public const string LAST_STAND = "LAST_STAND";

		public const string LARGEST_CRITICAL_STRIKE = "LARGEST_CRITICAL_STRIKE";

		public const string TOTAL_DAMAGE_DEALT = "TOTAL_DAMAGE_DEALT";

		public const string TOTAL_SCORE_RANK = "TOTAL_SCORE_RANK";

		public const string NODE_KILL_OFFENSE = "NODE_KILL_OFFENSE";

		public const string LARGEST_KILLING_SPREE = "LARGEST_KILLING_SPREE";

		public const string TURRETS_KILLED = "TURRETS_KILLED";

		public const string DEFENSE_PLAYER_SCORE = "DEFENSE_PLAYER_SCORE";

		public const string DEFENDE_POINT_NEUTRALIZE = "DEFENDE_POINT_NEUTRALIZE";

		public const string NODE_KILL_DEFENSE = "NODE_KILL_DEFENSE";

		public const string TOTAL_PLAYER_SCORE = "TOTAL_PLAYER_SCORE";

		public const string MINIONS_KILLED = "MINIONS_KILLED";

		public const string VICTORY_POINT_TOTAL = "VICTORY_POINT_TOTAL";

		public const string LEVEL = "LEVEL";

		public const string NODE_CAPTURE = "NODE_CAPTURE";

		public const string FIRST_BLOOD = "FIRST_BLOOD";

		public const string CHAMPION_KILLS = "CHAMPIONS_KILLED";

		public const string NEUTRAL_MINIONS_KILLED = "NEUTRAL_MINIONS_KILLED";

		public const string OBJECTIVE_PLAYER_SCORE = "OBJECTIVE_PLAYER_SCORE";

		public const string DEATHS = "NUM_DEATHS";

		[InternalName("statType")]
		public string StatType
		{
			get;
			set;
		}

		[InternalName("value")]
		public object Value
		{
			get;
			set;
		}

		public RawStat(ASObject obj) : base(obj)
		{
			BaseObject.SetFields<RawStat>(this, obj);
		}
	}
}