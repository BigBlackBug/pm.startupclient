using PlaymoreClient.Flash;
using PlaymoreClient.Messages;
using FluorineFx;
using System;
using System.Runtime.CompilerServices;

namespace PlaymoreClient.Messages.Account
{
	[Message(".Summoner")]
	public class SummonerData : BaseObject, ICloneable
	{
		[InternalName("acctId")]
		public long AccountId
		{
			get;
			set;
		}

		[InternalName("sumId")]
		public long SummonerId
		{
			get;
			set;
		}

		[InternalName("name")]
		public string Username
		{
			get;
			set;
		}

		public SummonerData() : base(null)
		{
		}

		public SummonerData(ASObject obj) : base(obj)
		{
			BaseObject.SetFields<SummonerData>(this, obj);
		}

		public object Clone()
		{
			SummonerData summonerDatum = new SummonerData();
			summonerDatum.Username = this.Username;
			summonerDatum.AccountId = this.AccountId;
			summonerDatum.SummonerId = this.SummonerId;
			return summonerDatum;
		}
	}
}