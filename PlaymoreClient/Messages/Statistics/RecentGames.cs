using PlaymoreClient.Flash;
using PlaymoreClient.Messages;
using FluorineFx;
using System;
using System.Runtime.CompilerServices;

namespace PlaymoreClient.Messages.Statistics
{
	[Message(".RecentGames")]
	public class RecentGames : MessageObject
	{
		[InternalName("gameStatistics")]
		public PlayerGameStatsList GameStatistics
		{
			get;
			set;
		}

		public RecentGames(ASObject obj) : base(obj)
		{
			this.GameStatistics = new PlayerGameStatsList();
			BaseObject.SetFields<RecentGames>(this, obj);
		}
	}
}