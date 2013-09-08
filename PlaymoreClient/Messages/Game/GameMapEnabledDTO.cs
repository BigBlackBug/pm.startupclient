using System;
using System.Runtime.CompilerServices;

namespace PlaymoreClient.Messages.Game
{
	public class GameMapEnabledDTO
	{
		public int gameMapId
		{
			get;
			set;
		}

		public int minPlayers
		{
			get;
			set;
		}

		public GameMapEnabledDTO()
		{
		}
	}
}