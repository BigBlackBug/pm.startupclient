using PlaymoreClient.Messaging.Messages;
using FluorineFx.AMF3;
using FluorineFx.Messaging.Api.Service;
using FluorineFx.Messaging.Messages;
using FluorineFx.Messaging.Rtmp.Event;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace PlaymoreClient.Util
{
	public static class RtmpUtil
	{
		[ThreadStatic]
		private static Random _rand;

		private static Random Rand
		{
			get
			{
				Random random = RtmpUtil._rand;
				if (random == null)
				{
					random = new Random();
					RtmpUtil._rand = random;
				}
				return random;
			}
		}

		public static string FromByteArray(ByteArray obj)
		{
			if (obj == null)
			{
				return null;
			}
			byte[] array = obj.MemoryStream.ToArray();
			if ((int)array.Length < 16)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < (int)array.Length; i++)
			{
				if (i == 4 || i == 6 || i == 8 || i == 10)
				{
					stringBuilder.Append('-');
				}
				stringBuilder.AppendFormat("{0:X2}", array[i]);
			}
			return stringBuilder.ToString();
		}

		public static IEnumerable<Tuple<object, long>> GetBodies(Notify notify)
		{
			List<Tuple<object, long>> tuples = new List<Tuple<object, long>>();
			object[] arguments = notify.ServiceCall.Arguments;
			for (int i = 0; i < (int)arguments.Length; i++)
			{
				object obj = arguments[i];
				Tuple<object, long> tuple = null;
				if (obj is AbstractMessage)
				{
					AbstractMessage abstractMessage = (AbstractMessage)obj;
					tuple = Tuple.Create<object, long>(abstractMessage.Body, abstractMessage.TimeStamp);
				}
				else if (obj is MessageBase)
				{
					MessageBase messageBase = (MessageBase)obj;
					tuple = Tuple.Create<object, long>(messageBase.body, messageBase.timestamp);
				}
				if (tuple != null)
				{
					tuples.Add(tuple);
				}
			}
			return tuples;
		}

		public static ErrorMessage GetError(Notify notify)
		{
			object[] arguments = notify.ServiceCall.Arguments;
			for (int i = 0; i < (int)arguments.Length; i++)
			{
				object obj = arguments[i];
				if (obj is ErrorMessage)
				{
					return (ErrorMessage)obj;
				}
			}
			return null;
		}

		public static bool IsError(Notify notify)
		{
			if (notify.ServiceCall == null)
			{
				return false;
			}
			return notify.ServiceCall.ServiceMethodName == "_error";
		}

		public static bool IsResult(Notify notify)
		{
			if (notify.ServiceCall == null)
			{
				return false;
			}
			if (notify.ServiceCall.ServiceMethodName == "_result")
			{
				return true;
			}
			return notify.ServiceCall.ServiceMethodName == "_error";
		}

		public static bool IsUid(string str)
		{
			if (str == null || str.Length != 36)
			{
				return false;
			}
			for (int i = 0; i < 36; i++)
			{
				char chr = str[i];
				if (i != 8 && i != 13 && i != 18 && i != 23)
				{
					if (chr < '0' || chr > 'F' || chr > '9' && chr < 'A')
					{
						return false;
					}
				}
				else if (chr != '-')
				{
					return false;
				}
			}
			return true;
		}

		public static ByteArray RandomUid()
		{
			byte[] numArray = new byte[16];
			RtmpUtil.Rand.NextBytes(numArray);
			return new ByteArray(numArray);
		}

		public static string RandomUidString()
		{
			return RtmpUtil.FromByteArray(RtmpUtil.RandomUid());
		}

		public static ByteArray ToByteArray(string str)
		{
			byte num;
			if (!RtmpUtil.IsUid(str))
			{
				return null;
			}
			str = str.Replace("-", "");
			ByteArray byteArray = new ByteArray();
			for (int i = 0; i < str.Length; i = i + 2)
			{
				if (!byte.TryParse(str.Substring(i, 2), NumberStyles.HexNumber, (IFormatProvider)null, out num))
				{
					return null;
				}
				byteArray.WriteByte(num);
			}
			byteArray.Position = 0;
			return byteArray;
		}
	}
}