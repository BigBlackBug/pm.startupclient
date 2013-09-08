using PlaymoreClient.Flash;
using FluorineFx;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace PlaymoreClient.Messages.GameLobby.Participants
{
	[DebuggerDisplay("{Name}")]
	public class GameParticipant : Participant
	{
		[InternalName("summonerInternalName")]
		public string InternalName
		{
			get;
			set;
		}

		public bool IsGameOwner
		{
			get;
			set;
		}

		public bool IsMe
		{
			get;
			set;
		}

		[InternalName("summonerName")]
		public string Name
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

		public GameParticipant() : base(null)
		{
		}

		public GameParticipant(ASObject thebase) : base(thebase)
		{
			BaseObject.SetFields<GameParticipant>(this, thebase);
		}

		public override object Clone()
		{
			GameParticipant gameParticipant = new GameParticipant();
			gameParticipant.InternalName = this.InternalName;
			gameParticipant.Name = this.Name;
			gameParticipant.PickMode = base.PickMode;
			gameParticipant.PickTurn = this.PickTurn;
			gameParticipant.IsGameOwner = this.IsGameOwner;
			gameParticipant.IsMe = this.IsMe;
			return gameParticipant;
		}

		public override bool Equals(object obj)
		{
			GameParticipant gameParticipant = obj as GameParticipant;
			if (gameParticipant == null)
			{
				return false;
			}
			return this.InternalName.Equals(gameParticipant.InternalName);
		}

		public override int GetHashCode()
		{
			return this.InternalName.GetHashCode();
		}
	}
}