using FluorineFx.AMF3;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PlaymoreClient.Messaging.Messages
{
	public class CommandMessage : AsyncMessage
	{
		public const byte OPERATION_FLAG = 1;

		public const uint SUBSCRIBE_OPERATION = 0;

		public const uint CLIENT_SYNC_OPERATION = 4;

		public const uint SUBSCRIPTION_INVALIDATE_OPERATION = 10;

		public const uint TRIGGER_CONNECT_OPERATION = 13;

		public const uint CLIENT_PING_OPERATION = 5;

		public const uint UNSUBSCRIBE_OPERATION = 1;

		public const uint POLL_OPERATION = 2;

		public const uint MULTI_SUBSCRIBE_OPERATION = 11;

		public const uint LOGIN_OPERATION = 8;

		public const uint CLUSTER_REQUEST_OPERATION = 7;

		public const uint LOGOUT_OPERATION = 9;

		public const uint UNKNOWN_OPERATION = 10000;

		public const uint DISCONNECT_OPERATION = 12;

		private static Dictionary<uint, string> operationtexts;

		public uint Operation
		{
			get;
			set;
		}

		public string OperationString
		{
			get;
			set;
		}

		static CommandMessage()
		{
			CommandMessage.operationtexts = new Dictionary<uint, string>();
			CommandMessage.operationtexts[0] = "subscribe";
			CommandMessage.operationtexts[1] = "unsubscribe";
			CommandMessage.operationtexts[2] = "poll";
			CommandMessage.operationtexts[4] = "client sync";
			CommandMessage.operationtexts[5] = "client ping";
			CommandMessage.operationtexts[7] = "cluster request";
			CommandMessage.operationtexts[8] = "login";
			CommandMessage.operationtexts[9] = "logout";
			CommandMessage.operationtexts[10] = "subscription invalidate";
			CommandMessage.operationtexts[11] = "multi-subscribe";
			CommandMessage.operationtexts[12] = "disconnect";
			CommandMessage.operationtexts[13] = "trigger connect";
			CommandMessage.operationtexts[10000] = "unknown";
		}

		public CommandMessage()
		{
		}

		public override void ReadExternal(IDataInput input)
		{
			base.ReadExternal(input);
			List<byte> nums = AbstractMessage.ReadFlags(input);
			for (int i = 0; i < nums.Count; i++)
			{
				int num = 0;
				if (i == 0)
				{
					if ((nums[i] & 1) != 0)
					{
						this.SetOperation((uint)input.ReadObject());
					}
					num = 1;
				}
				base.ReadRemaining(input, (int)nums[i], num);
			}
		}

		protected void SetOperation(uint num)
		{
			string str;
			string str1;
			this.Operation = num;
			str1 = (CommandMessage.operationtexts.TryGetValue(num, out str) ? str : num.ToString());
			this.OperationString = str1;
		}

		public override void WriteExternal(IDataOutput output)
		{
			base.WriteExternal(output);
			int num = 0;
			if (this.Operation != 0)
			{
				num = num | 1;
			}
			output.WriteByte((byte)num);
			if (this.Operation != 0)
			{
				output.WriteObject(this.Operation);
			}
		}
	}
}