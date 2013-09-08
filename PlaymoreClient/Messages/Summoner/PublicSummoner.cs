using PlaymoreClient.Flash;
using PlaymoreClient.Messages;
using FluorineFx;
using System;
using System.Runtime.CompilerServices;

namespace PlaymoreClient.Messages.Summoner
{
	[Message(".PublicSummoner")]
	public class PublicSummoner : MessageObject
	{
		[InternalName("acctId")]
		public long AccountId
		{
			get;
			set;
		}

		[InternalName("dataVersion")]
		public int DataVersion
		{
			get;
			set;
		}

		[InternalName("internalName")]
		public string InternalName
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

		[InternalName("summonerLevel")]
		public int SummonerLevel
		{
			get;
			set;
		}

		public PublicSummoner(ASObject obj) : base(obj)
		{
			BaseObject.SetFields<PublicSummoner>(this, obj);
		}
	}
}