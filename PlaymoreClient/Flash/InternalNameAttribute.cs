using System;
using System.Runtime.CompilerServices;

namespace PlaymoreClient.Flash
{
	public class InternalNameAttribute : Attribute
	{
		public string Name
		{
			get;
			set;
		}

		public InternalNameAttribute(string name)
		{
			this.Name = name;
		}
	}
}