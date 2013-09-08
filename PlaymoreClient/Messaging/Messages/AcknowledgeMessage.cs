using FluorineFx.AMF3;
using System;
using System.Collections.Generic;

namespace PlaymoreClient.Messaging.Messages
{
	public class AcknowledgeMessage : AsyncMessage
	{
		public AcknowledgeMessage()
		{
		}

		public override void ReadExternal(IDataInput input)
		{
			base.ReadExternal(input);
			List<byte> nums = AbstractMessage.ReadFlags(input);
			for (int i = 0; i < nums.Count; i++)
			{
				base.ReadRemaining(input, (int)nums[i], 0);
			}
		}

		public override void WriteExternal(IDataOutput output)
		{
			base.WriteExternal(output);
			output.WriteByte(0);
		}
	}
}