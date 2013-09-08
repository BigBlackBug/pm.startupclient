using PlaymoreClient.Messages.GameLobby.Participants;
using FluorineFx;
using FluorineFx.AMF3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace PlaymoreClient.Messages.GameLobby
{
	[DebuggerDisplay("Count: {Count}")]
	public class TeamParticipants : List<Participant>
	{
		protected readonly ArrayCollection Base;

		public TeamParticipants()
		{
		}

		public TeamParticipants(IEnumerable<Participant> collection) : base(collection)
		{
		}
        public override string ToString()
        {
            string str = "";

            foreach (object p in base.ToArray())
            {
                
                str += "("+p.ToString()+")";
            }
            return "{"+str+"}";
        }

		public TeamParticipants(ArrayCollection thebase)
		{
			this.Base = thebase;
			if (this.Base == null)
			{
				return;
			}
			foreach (object @base in this.Base)
			{
				ASObject aSObject = @base as ASObject;
				if (aSObject == null)
				{
					continue;
				}
				if (aSObject.TypeName.Contains("PlayerParticipant"))
				{
					base.Add(new PlayerParticipant(aSObject));
				}
				else if (!aSObject.TypeName.Contains("ObfuscatedParticipant"))
				{
					if (!aSObject.TypeName.Contains("BotParticipant"))
					{
						throw new NotSupportedException(string.Concat("Unexcepted type in team array ", aSObject.TypeName));
					}
					base.Add(new BotParticipant(aSObject));
				}
				else
				{
					base.Add(new ObfuscatedParticipant(aSObject));
				}
			}
		}
	}
}