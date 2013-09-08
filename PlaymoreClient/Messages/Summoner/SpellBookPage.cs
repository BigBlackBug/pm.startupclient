using PlaymoreClient.Flash;
using PlaymoreClient.Messages;
using FluorineFx;
using System;
using System.Runtime.CompilerServices;

namespace PlaymoreClient.Messages.Summoner
{
	[Message(".SpellBookPage", "com.riotgames.platform.summoner.spellbook.SpellBookPage")]
	public class SpellBookPage : MessageObject
	{
		[InternalName("createDate")]
		public DateTime CreateDate
		{
			get;
			set;
		}

		[InternalName("isCurrent")]
		public bool IsCurrent
		{
			get;
			set;
		}

		[InternalName("name")]
		public string Name
		{
			get;
			set;
		}

		[InternalName("pageId")]
		public int PageId
		{
			get;
			set;
		}

		[InternalName("slotEntries")]
		public SlotEntryList SlotEntries
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

		public SpellBookPage() : base(null)
		{
			this.SlotEntries = new SlotEntryList();
		}

		public SpellBookPage(ASObject obj) : base(obj)
		{
			BaseObject.SetFields<SpellBookPage>(this, obj);
		}
	}
}