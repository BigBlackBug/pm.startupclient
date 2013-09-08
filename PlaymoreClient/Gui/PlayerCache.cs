using PlaymoreClient.Messages.Statistics;
using PlaymoreClient.Messages.Summoner;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace PlaymoreClient.Gui
{
	public class PlayerCache
	{
		private readonly ManualResetEvent m_LoadWait;

		public RecentGames Games
		{
			get;
			set;
		}

		public ManualResetEvent LoadWait
		{
			get
			{
				return this.m_LoadWait;
			}
		}

		public ChampionStatInfoList RecentChamps
		{
			get;
			set;
		}

		public int SeenCount
		{
			get;
			set;
		}

		public PlayerLifetimeStats Stats
		{
			get;
			set;
		}

		public PublicSummoner Summoner
		{
			get;
			set;
		}

		public PlayerCache()
		{
			this.m_LoadWait = new ManualResetEvent(false);
		}
	}
}