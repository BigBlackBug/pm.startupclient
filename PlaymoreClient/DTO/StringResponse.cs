using System;
using System.Runtime.CompilerServices;

namespace PlaymoreClient.DTO
{
	public class StringResponse
	{
		public string data
		{
			get;
			set;
		}

		public string error
		{
			get;
			set;
		}

		public bool success
		{
			get;
			set;
		}

		public StringResponse()
		{
		}
	}
}