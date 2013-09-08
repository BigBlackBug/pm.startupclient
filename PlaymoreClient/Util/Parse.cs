using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace PlaymoreClient.Util
{
	public static class Parse
	{
		public static bool Bool(string str)
		{
			bool flag;
			if (!bool.TryParse(str, out flag))
			{
				throw new FormatException(string.Format("Expected {0} got {1}", flag.GetType(), str));
			}
			return flag;
		}

		public static DateTime Date(string str)
		{
			DateTime dateTime;
			Match match = Regex.Match(str, "\\w+ (?<month>\\w+) (?<day>\\d+) (?<hour>\\d+):(?<minute>\\d+):(?<second>\\d+) GMT(?<gmt>[+-]\\d\\d)\\d* (?<year>\\d+)");
			if (!match.Success)
			{
				throw new FormatException(string.Concat("Invalid date format ", str));
			}
			object[] item = new object[] { match.Groups["month"], match.Groups["day"], match.Groups["hour"], match.Groups["minute"], match.Groups["second"], match.Groups["gmt"], match.Groups["year"] };
			if (!DateTime.TryParseExact(string.Format("{0} {1} {2} {3} {4} {5} {6}", item), "MMM dd HH mm ss zz yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out dateTime))
			{
				throw new FormatException(string.Concat("Invalid date format ", str));
			}
			return dateTime;
		}

		public static int Int(string str)
		{
			int num;
			if (str == "NaN")
			{
				return 0;
			}
			if (!int.TryParse(str, out num))
			{
				throw new FormatException(string.Format("Expected {0} got {1}", num.GetType(), str));
			}
			return num;
		}

		public static long Long(string str)
		{
			long num;
			if (str == "NaN")
			{
				return (long)0;
			}
			if (!long.TryParse(str, out num))
			{
				throw new FormatException(string.Format("Expected {0} got {1}", num.GetType(), str));
			}
			return num;
		}

		public static string ToBase64(string str)
		{
			if (str == null)
			{
				return null;
			}
			return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
		}
	}
}