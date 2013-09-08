using PlaymoreClient.Flash;
using FluorineFx;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace PlaymoreClient.Messages.GameLobby.Participants
{
	[DebuggerDisplay("{Name}")]
	public class PlayerParticipant : GameParticipant
	{
		[InternalName("accountId")]
		public long AccountId
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

		[InternalName("summonerId")]
		public long SummonerId
		{
			get;
			set;
		}

		[InternalName("teamParticipantId")]
		public long TeamParticipantId
		{
			get;
			set;
		}

		public PlayerParticipant() : base(null)
		{
		}

		public PlayerParticipant(ASObject thebase) : base(thebase)
		{
			BaseObject.SetFields<PlayerParticipant>(this, thebase);
		}

		public override object Clone()
		{
			PlayerParticipant playerParticipant = new PlayerParticipant();
			playerParticipant.ProfileIconId = this.ProfileIconId;
			playerParticipant.SummonerId = this.SummonerId;
			playerParticipant.InternalName = base.InternalName;
			playerParticipant.IsMe = base.IsMe;
			playerParticipant.IsGameOwner = base.IsGameOwner;
			playerParticipant.Name = base.Name;
			playerParticipant.PickMode = base.PickMode;
			playerParticipant.PickTurn = base.PickTurn;
			playerParticipant.TeamParticipantId = this.TeamParticipantId;
			return playerParticipant;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (!(obj is PlayerParticipant))
			{
				return base.Equals(obj);
			}
			return this.SummonerId == ((PlayerParticipant)obj).SummonerId;
		}

        public override string ToString()
        {
            return base.ToString() + "summ_id " + SummonerId + " summ_int_name " + InternalName + " summ_name " + Name + " acc_id " + AccountId + " team_participant_id " + TeamParticipantId; ;
        }
	}
}