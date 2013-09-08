using FluorineFx.AMF3;
using System;
using System.Runtime.CompilerServices;

namespace com.riotgames.platform.broadcast
{
	public class BroadcastNotification : IExternalizable
	{
		public string Json
		{
			get;
			set;
		}

		public BroadcastNotification()
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