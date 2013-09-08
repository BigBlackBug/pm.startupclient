using PlaymoreClient.Flash;
using FluorineFx;
using System;
using System.Runtime.CompilerServices;

namespace PlaymoreClient.Messages
{
	public class MessageObject : BaseObject
	{
		public long TimeStamp
		{
			get;
			set;
		}

        public string ObjectType
        {
            get;
            set;
        }

		public MessageObject(ASObject obj) : base(obj)
		{
			BaseObject.SetFields<MessageObject>(this, obj);
		}
	}
}