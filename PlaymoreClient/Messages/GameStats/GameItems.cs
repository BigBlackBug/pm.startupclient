using FluorineFx.AMF3;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PlaymoreClient.Messages.GameStats
{
	public class GameItems : List<int>
	{
		protected readonly ArrayCollection Base;

		public GameItems()
		{
		}

		public GameItems(ArrayCollection body)
		{
			if (body == null)
			{
				throw new ArgumentNullException("body");
			}
			this.Base = body;
			foreach (object obj in body)
			{
				base.Add(Convert.ToInt32(obj));
			}
		}
	}
}