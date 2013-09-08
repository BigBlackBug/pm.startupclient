using PlaymoreClient.Flash;
using FluorineFx;
using System;
using System.Runtime.CompilerServices;

namespace PlaymoreClient.Messages.GameLobby.Participants
{
	public class Participant : BaseObject, ICloneable
	{
		[InternalName("pickMode")]
		public int PickMode
		{
			get;
			set;
		}

		public Participant() : base(null)
		{
		}

		public Participant(ASObject thebase) : base(thebase)
		{
			BaseObject.SetFields<Participant>(this, thebase);
		}

		public virtual object Clone()
		{
			Participant participant = new Participant();
			participant.PickMode = this.PickMode;
			return participant;
		}
	}
}