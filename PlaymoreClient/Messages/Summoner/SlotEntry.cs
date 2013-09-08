using PlaymoreClient.Flash;
using PlaymoreClient.Messages;
using FluorineFx;
using System;
using System.Runtime.CompilerServices;

namespace PlaymoreClient.Messages.Summoner
{
	[Message(".SlotEntry", "com.riotgames.platform.summoner.spellbook.SlotEntry")]
	public class SlotEntry : MessageObject
	{
		[InternalName("runeId")]
		public int RuneId
		{
			get;
			set;
		}

		[InternalName("runeSlotId")]
		public int RuneSlotId
		{
			get;
			set;
		}

		public SlotEntry() : base(null)
		{
		}

		public SlotEntry(ASObject obj) : base(obj)
		{
			BaseObject.SetFields<SlotEntry>(this, obj);
		}
	}
}