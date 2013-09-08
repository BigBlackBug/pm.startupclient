using PlaymoreClient.Flash;
using PlaymoreClient.Messages;
using FluorineFx;
using NotMissing;
using System;
using System.Runtime.CompilerServices;

namespace PlaymoreClient.Messages.Account
{
	[Message(".LoginDataPacket")]
	public class LoginDataPacket : BaseObject, ICloneable
	{
		[InternalName("allSummonerData")]
		public PlaymoreClient.Messages.Account.AllSummonerData AllSummonerData
		{
			get;
			set;
		}

		public LoginDataPacket() : base(null)
		{
		}

		public LoginDataPacket(ASObject obj) : base(obj)
		{
			BaseObject.SetFields<LoginDataPacket>(this, obj);
		}

		public object Clone()
		{
			LoginDataPacket loginDataPacket = new LoginDataPacket();
			loginDataPacket.AllSummonerData = this.AllSummonerData.CloneT<PlaymoreClient.Messages.Account.AllSummonerData>();
			return loginDataPacket;
		}
	}
}