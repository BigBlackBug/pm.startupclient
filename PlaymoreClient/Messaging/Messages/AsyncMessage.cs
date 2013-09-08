using PlaymoreClient.Util;
using FluorineFx.AMF3;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PlaymoreClient.Messaging.Messages
{
	public class AsyncMessage : AbstractMessage
    {
		private const byte CORRELATION_ID_FLAG = 1;

		private const byte CORRELATION_ID_BYTES_FLAG = 2;

		public string CorrelationId
		{
			get;
			set;
		}

		public ByteArray CorrelationIdBytes
		{
			get;
			set;
		}

		public AsyncMessage()
		{
		}

		public override void ReadExternal(IDataInput input)
		{
            PlaymoreClient.Gui.MainForm.LOGGER.Info("reading async message");
			base.ReadExternal(input);
			List<byte> nums = AbstractMessage.ReadFlags(input);
			for (int i = 0; i < nums.Count; i++)
			{
				int num = 0;
				if (i == 0)
				{
					if ((nums[i] & 1) != 0)
					{
						this.CorrelationId = input.ReadObject() as string;
					}
					if ((nums[i] & 2) != 0)
					{
						this.CorrelationId = RtmpUtil.FromByteArray(input.ReadObject() as ByteArray);
					}
					num = 2;
				}
				base.ReadRemaining(input, (int)nums[i], num);
			}
		}

		public override void WriteExternal(IDataOutput output)
		{
			base.WriteExternal(output);
			if (this.CorrelationIdBytes == null)
			{
				this.CorrelationIdBytes = RtmpUtil.ToByteArray(this.CorrelationId);
			}
			int num = 0;
			if (this.CorrelationId != null && this.CorrelationIdBytes == null)
			{
				num = num | 1;
			}
			if (this.CorrelationIdBytes != null)
			{
				num = num | 2;
			}
			output.WriteByte((byte)num);
			if (this.CorrelationId != null && this.CorrelationIdBytes == null)
			{
				output.WriteObject(this.CorrelationId);
			}
			if (this.CorrelationIdBytes != null)
			{
				output.WriteObject(this.CorrelationIdBytes);
			}
		}
	}
}