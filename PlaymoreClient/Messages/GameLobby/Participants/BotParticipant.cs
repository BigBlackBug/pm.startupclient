using PlaymoreClient.Flash;
using FluorineFx;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace PlaymoreClient.Messages.GameLobby.Participants
{
	[DebuggerDisplay("{Name}")]
	public class BotParticipant : GameParticipant
	{
		[InternalName("botSkillLevel")]
		public int BotSkillLevel
		{
			get;
			set;
		}

		public string BotSkillLevelName
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

		public BotParticipant() : base(null)
		{
		}

		public BotParticipant(ASObject thebase) : base(thebase)
		{
			BaseObject.SetFields<BotParticipant>(this, thebase);
		}

		public override object Clone()
		{
			BotParticipant botParticipant = new BotParticipant();
			botParticipant.Name = base.Name;
			botParticipant.InternalName = base.InternalName;
			botParticipant.PickMode = base.PickMode;
			botParticipant.IsGameOwner = base.IsGameOwner;
			botParticipant.PickTurn = base.PickTurn;
			botParticipant.IsMe = base.IsMe;
			botParticipant.BotSkillLevelName = this.BotSkillLevelName;
			botParticipant.BotSkillLevel = this.BotSkillLevel;
			botParticipant.TeamId = this.TeamId;
			return botParticipant;
		}
	}
}