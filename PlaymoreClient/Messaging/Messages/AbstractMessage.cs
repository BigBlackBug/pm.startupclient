using PlaymoreClient.Util;
using FluorineFx.AMF3;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace PlaymoreClient.Messaging.Messages
{
	public class AbstractMessage : IExternalizable
	{
		public const byte HAS_NEXT_FLAG = 128;

		public const byte MESSAGE_ID_FLAG = 16;

		public const byte TIME_TO_LIVE_FLAG = 64;

		public const byte TIMESTAMP_FLAG = 32;

		public const byte CLIENT_ID_BYTES_FLAG = 1;

		public const byte DESTINATION_FLAG = 4;

		public const byte CLIENT_ID_FLAG = 2;

		public const byte HEADERS_FLAG = 8;

		public const byte BODY_FLAG = 1;

		public const byte MESSAGE_ID_BYTES_FLAG = 2;

		public object Body
		{
			get;
			set;
		}

		public string ClientId
		{
			get;
			set;
		}

		public ByteArray ClientIdBytes
		{
			get;
			set;
		}

		public string Destination
		{
			get;
			set;
		}

		public object Headers
		{
			get;
			set;
		}

		public string MessageId
		{
			get;
			set;
		}

		public ByteArray MessageIdBytes
		{
			get;
			set;
		}

		public long TimeStamp
		{
			get;
			set;
		}

		public long TimeToLive
		{
			get;
			set;
		}

		public AbstractMessage()
		{
		}

		public virtual void ReadExternal(IDataInput input)
		{
			List<byte> nums = AbstractMessage.ReadFlags(input);
			for (int i = 0; i < nums.Count; i++)
			{
				int num = 0;
				if (i == 0)
				{
					if ((nums[i] & 1) != 0)
					{
						this.Body = input.ReadObject();
					}
					if ((nums[i] & 2) != 0)
					{
						this.ClientId = input.ReadObject() as string;
					}
					if ((nums[i] & 4) != 0)
					{
						this.Destination = input.ReadObject() as string;
					}
					if ((nums[i] & 8) != 0)
					{
						this.Headers = input.ReadObject();
					}
					if ((nums[i] & 16) != 0)
					{
						this.MessageId = input.ReadObject() as string;
					}
					if ((nums[i] & 32) != 0)
					{
						this.TimeStamp = Convert.ToInt64(input.ReadObject());
					}
					if ((nums[i] & 64) != 0)
					{
						this.TimeToLive = Convert.ToInt64(input.ReadObject());
					}
					num = 7;
				}
				else if (i == 1)
				{
					if ((nums[i] & 1) != 0)
					{
						this.ClientIdBytes = input.ReadObject() as ByteArray;
						this.ClientId = RtmpUtil.FromByteArray(this.ClientIdBytes);
					}
					if ((nums[i] & 2) != 0)
					{
						this.MessageIdBytes = input.ReadObject() as ByteArray;
						this.MessageId = RtmpUtil.FromByteArray(this.MessageIdBytes);
					}
					num = 2;
				}
				this.ReadRemaining(input, (int)nums[i], num);
			}
		}

		public static List<byte> ReadFlags(IDataInput input)
		{
			byte num;
			List<byte> nums = new List<byte>();
			do
			{
				byte num1 = input.ReadUnsignedByte();
				num = num1;
				nums.Add(num1);
			}
			while ((num & 128) != 0);
			return nums;
		}

		protected void ReadRemaining(IDataInput input, int flag, int bits)
		{
			if (flag >> (bits & 31) != 0)
			{
				for (int i = bits; i < 6; i++)
				{
					if ((flag >> (i & 31) & 1) != 0)
					{
						input.ReadObject();
					}
				}
			}
		}

		public override string ToString()
		{
			return string.Format("{0}({1})", (!string.IsNullOrEmpty(this.Destination) ? string.Concat(this.Destination, " ") : ""), this.GetType().Name);
		}

		public virtual void WriteExternal(IDataOutput output)
		{
			if (this.ClientIdBytes == null)
			{
				this.ClientIdBytes = RtmpUtil.ToByteArray(this.ClientId);
			}
			if (this.MessageIdBytes == null)
			{
				this.MessageIdBytes = RtmpUtil.ToByteArray(this.MessageId);
			}
			int num = 0;
			if (this.Body != null)
			{
				num = num | 1;
			}
			if (this.ClientId != null && this.ClientIdBytes == null)
			{
				num = num | 2;
			}
			if (this.Destination != null)
			{
				num = num | 4;
			}
			if (this.Headers != null)
			{
				num = num | 8;
			}
			if (this.MessageId != null && this.MessageIdBytes == null)
			{
				num = num | 16;
			}
			if (this.TimeStamp != (long)0)
			{
				num = num | 32;
			}
			if (this.TimeToLive != (long)0)
			{
				num = num | 64;
			}
			int num1 = 0;
			if (this.ClientIdBytes != null)
			{
				num1 = num1 | 1;
			}
			if (this.MessageIdBytes != null)
			{
				num1 = num1 | 2;
			}
			if (num1 != 0)
			{
				num = num | 128;
			}
			output.WriteByte((byte)num);
			if (num1 != 0)
			{
				output.WriteByte((byte)num1);
			}
			if (this.Body != null)
			{
				output.WriteObject(this.Body);
			}
			if (this.ClientId != null && this.ClientIdBytes == null)
			{
				output.WriteObject(this.ClientId);
			}
			if (this.Destination != null)
			{
				output.WriteObject(this.Destination);
			}
			if (this.Headers != null)
			{
				output.WriteObject(this.Headers);
			}
			if (this.MessageId != null && this.MessageIdBytes == null)
			{
				output.WriteObject(this.MessageId);
			}
			if (this.TimeStamp != (long)0)
			{
				output.WriteObject(this.TimeStamp);
			}
			if (this.TimeToLive != (long)0)
			{
				output.WriteObject(this.TimeToLive);
			}
			if (this.ClientIdBytes != null)
			{
				output.WriteObject(this.ClientIdBytes);
			}
			if (this.MessageIdBytes != null)
			{
				output.WriteObject(this.MessageIdBytes);
			}
		}
	}
}