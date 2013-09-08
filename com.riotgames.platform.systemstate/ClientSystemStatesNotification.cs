using FluorineFx.AMF3;
using System;
using System.Runtime.CompilerServices;

namespace com.riotgames.platform.systemstate
{
	public class ClientSystemStatesNotification : IExternalizable
	{
		public string Json
		{
			get;
			set;
		}

		public ClientSystemStatesNotification()
		{
		}

		public void ReadExternal(IDataInput input)
		{
			this.Json = input.ReadUTFBytes(input.ReadUnsignedInt());
		}

		public void WriteExternal(IDataOutput output)
		{
			output.WriteInt(this.Json.Length);
			output.WriteUTFBytes(this.Json);
		}
	}
}