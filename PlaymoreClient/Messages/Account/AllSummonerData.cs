using PlaymoreClient.Flash;
using PlaymoreClient.Messages;
using FluorineFx;
using NotMissing;
using System;
using System.Runtime.CompilerServices;

namespace PlaymoreClient.Messages.Account
{
	[Message(".AllSummonerData")]
	public class AllSummonerData : BaseObject, ICloneable
	{
		[InternalName("summoner")]
		public SummonerData Summoner
		{
			get;
			set;
		}

		public AllSummonerData() : base(null)
		{
		}

		public AllSummonerData(ASObject obj) : base(obj)
		{
			BaseObject.SetFields<AllSummonerData>(this, obj);
		}

		public object Clone()
		{
			AllSummonerData allSummonerDatum = new AllSummonerData();
			allSummonerDatum.Summoner = this.Summoner.CloneT<SummonerData>();
			return allSummonerDatum;
		}
	}
}