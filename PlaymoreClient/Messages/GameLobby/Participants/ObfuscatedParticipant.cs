using PlaymoreClient.Flash;
using FluorineFx;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace PlaymoreClient.Messages.GameLobby.Participants
{
	[DebuggerDisplay("Id: {GameUniqueId}")]
	public class ObfuscatedParticipant : Participant
	{
		[InternalName("gameUniqueId")]
		public long GameUniqueId
		{
			get;
			set;
		}

		public ObfuscatedParticipant() : base(null)
		{
		}

		public ObfuscatedParticipant(ASObject thebase) : base(thebase)
		{
			BaseObject.SetFields<ObfuscatedParticipant>(this, thebase);
		}

		public override object Clone()
		{
			ObfuscatedParticipant obfuscatedParticipant = new ObfuscatedParticipant();
			obfuscatedParticipant.GameUniqueId = this.GameUniqueId;
			obfuscatedParticipant.PickMode = base.PickMode;
			return obfuscatedParticipant;
		}

		public override bool Equals(object obj)
		{
			ObfuscatedParticipant obfuscatedParticipant = obj as ObfuscatedParticipant;
			if (obfuscatedParticipant == null)
			{
				return false;
			}
			return this.GameUniqueId == obfuscatedParticipant.GameUniqueId;
		}

		public override int GetHashCode()
		{
			return (int)this.GameUniqueId;
		}
	}
}