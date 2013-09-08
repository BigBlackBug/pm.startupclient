using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace PlaymoreClient.Flash
{
	public class FlashSerializer
	{
		public FlashSerializer()
		{
		}

		private static char DecodeEscaped(char c)
		{
			char chr = c;
			if (chr > 'f')
			{
				if (chr == 'n')
				{
					return '\n';
				}
				switch (chr)
				{
					case 'r':
					{
						return '\r';
					}
					case 't':
					{
						return '\t';
					}
				}
			}
			else
			{
				if (chr == 'b')
				{
					return '\b';
				}
				if (chr == 'f')
				{
					return '\f';
				}
			}
			return c;
		}

		public static FlashObject Deserialize(StreamReader reader)
		{
			FlashObject flashObject = new FlashObject("Base");
			FlashObject parent = flashObject;
			Stack<int> nums = new Stack<int>();
			nums.Push(0);
			while (nums.Count > 0)
			{
				if (reader.Peek() != 32)
				{
					return flashObject;
				}
				string str = reader.ReadLine();
				FlashSerializer.KeyValue keyValue = FlashSerializer.MatchLine(str);
				if (keyValue == null)
				{
					throw new NotSupportedException(string.Concat("Unable to parse (", str, ")"));
				}
				while (nums.Count > 0 && FlashSerializer.GetLevel(str) <= nums.Peek())
				{
					parent = parent.Parent;
					nums.Pop();
				}
				string objectName = FlashSerializer.GetObjectName(str);
				if (objectName == null)
				{
					if (keyValue.Value.Length > 0 && keyValue.Value[0] == '\"')
					{
						string str1 = FlashSerializer.ParseString(keyValue.Value.Substring(1));
						if (str1 == null)
						{
							keyValue.Value = string.Concat(keyValue.Value.Substring(1), FlashSerializer.ParseString(reader));
							reader.ReadLine();
						}
						else
						{
							keyValue.Value = str1;
						}
					}
					parent[keyValue.Key] = new FlashObject(keyValue.Key, keyValue.Value);
				}
				else
				{
					FlashObject flashObject1 = new FlashObject(objectName, keyValue.Key, keyValue.Value);
					flashObject1.Parent = parent;
					FlashObject flashObject2 = flashObject1;
					parent[keyValue.Key] = flashObject2;
					parent = flashObject2;
					nums.Push(FlashSerializer.GetLevel(str));
				}
			}
			return flashObject;
		}

		private static int GetLevel(string str)
		{
			int num = 0;
			for (int i = 0; i < str.Length && str[i] == ' '; i++)
			{
				num++;
			}
			return num / 2;
		}

		private static string GetObjectName(string str)
		{
			Match match = Regex.Match(str, "\\((.*)\\)#\\d+$");
			if (!match.Success)
			{
				return null;
			}
			return match.Groups[1].Value;
		}

		private static FlashSerializer.KeyValue MatchLine(string str)
		{
			str = str.TrimStart(new char[] { ' ' });
			if (str.Length < 1)
			{
				return null;
			}
			Match match = Regex.Match(str, (str[0] == '[' ? "^\\[?(.+?)]?\\s(.+)$" : "^(.+)?\\s=\\s(.+)?$"));
			if (!match.Success)
			{
				return null;
			}
			return new FlashSerializer.KeyValue(match.Groups[1].Value, match.Groups[2].Value);
		}

		private static string ParseString(string str)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (str.Length < 1)
			{
				return str;
			}
			for (int i = 0; i < str.Length; i++)
			{
				if (str[i] == '\"')
				{
					return stringBuilder.ToString();
				}
				if (str[i] != '\\')
				{
					stringBuilder.Append(str[i]);
				}
				else
				{
					int num = i + 1;
					i = num;
					stringBuilder.Append(FlashSerializer.DecodeEscaped(str[num]));
				}
			}
			return null;
		}

		private static string ParseString(StreamReader reader)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < 100000; i++)
			{
				char chr = (char)FlashSerializer.ReadByte(reader);
				if (chr == '\"')
				{
					return stringBuilder.ToString();
				}
				if (chr != '\\')
				{
					stringBuilder.Append(chr);
				}
				else
				{
					stringBuilder.Append(FlashSerializer.DecodeEscaped((char)FlashSerializer.ReadByte(reader)));
				}
			}
			throw new Exception("String exceeds max size allowed");
		}

		private static int ReadByte(StreamReader reader)
		{
			int num = reader.Read();
			if (num == -1)
			{
				throw new EndOfStreamException();
			}
			return num;
		}

		[DebuggerDisplay("{Key}: {Value}")]
		private class KeyValue
		{
			public string Key
			{
				get;
				set;
			}

			public string Value
			{
				get;
				set;
			}

			public KeyValue(string key, string value)
			{
				this.Key = key;
				this.Value = value;
			}
		}
	}
}