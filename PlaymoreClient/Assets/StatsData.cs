using PlaymoreClient.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace PlaymoreClient.Assets
{
	public class StatsData : Dictionary<string, string>
	{
		protected readonly static StatsData _instance;

		public static StatsData Instance
		{
			get
			{
				return StatsData._instance;
			}
		}

		static StatsData()
		{
			StatsData._instance = JsonConvert.DeserializeObject<StatsData>(Resources.StatsData);
		}

		public StatsData()
		{
		}

		public static string Get(string key)
		{
			string str;
			if (!StatsData._instance.TryGetValue(key, out str))
			{
				return key;
			}
			return str;
		}
	}
}