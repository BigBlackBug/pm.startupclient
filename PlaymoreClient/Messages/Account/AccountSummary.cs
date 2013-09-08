using PlaymoreClient.Flash;
using PlaymoreClient.Messages;
using FluorineFx;
using System;
using System.Runtime.CompilerServices;

namespace PlaymoreClient.Messages.Account
{
	[Message(".AccountSummary")]
	public class AccountSummary : BaseObject
	{
		[InternalName("accountId")]
		public long AccountId
		{
			get;
			set;
		}

		[InternalName("username")]
		public string Username
		{
			get;
			set;
		}

		public AccountSummary(ASObject obj) : base(obj)
		{
			BaseObject.SetFields<AccountSummary>(this, obj);
		}
	}
}