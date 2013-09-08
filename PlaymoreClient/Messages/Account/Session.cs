using PlaymoreClient.Flash;
using PlaymoreClient.Messages;
using FluorineFx;
using System;
using System.Runtime.CompilerServices;

namespace PlaymoreClient.Messages.Account
{
	[Message(".Session")]
	public class Session : BaseObject
	{
		[InternalName("accountSummary")]
		public AccountSummary Account
		{
			get;
			set;
		}

		public Session(ASObject obj) : base(obj)
		{
			BaseObject.SetFields<Session>(this, obj);
		}
	}
}