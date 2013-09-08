using PlaymoreClient.Flash;
using FluorineFx;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace PlaymoreClient.Messages.GameStats.PlayerStats
{
	[DebuggerDisplay("{DisplayName}")]
	public class PlayerStatCategory : BaseObject, ICloneable
	{
		[InternalName("displayName")]
		public string DisplayName
		{
			get;
			set;
		}

		[InternalName("name")]
		public string Name
		{
			get;
			set;
		}

		[InternalName("priority")]
		public int Priority
		{
			get;
			set;
		}

		public PlayerStatCategory() : base(null)
		{
		}

		public PlayerStatCategory(ASObject thebase) : base(thebase)
		{
			BaseObject.SetFields<PlayerStatCategory>(this, this.Base);
		}

		public object Clone()
		{
			PlayerStatCategory playerStatCategory = new PlayerStatCategory();
			playerStatCategory.DisplayName = this.DisplayName;
			playerStatCategory.Name = this.Name;
			playerStatCategory.Priority = this.Priority;
			return playerStatCategory;
		}
	}
}