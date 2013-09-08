using System;
using System.Runtime.CompilerServices;

namespace PlaymoreClient.Messages
{
	public class MessageAttribute : Attribute
	{
		public string FullName
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public MessageAttribute(string name)
		{
			this.Name = name;
		}

		public MessageAttribute(string name, string fullName)
		{
			this.FullName = fullName;
			this.Name = name;
		}
	}
}