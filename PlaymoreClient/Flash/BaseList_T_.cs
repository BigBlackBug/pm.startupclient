using FluorineFx.AMF3;
using System;
using System.Collections.Generic;

namespace PlaymoreClient.Flash
{
	public class BaseList<T> : List<T>
	{
		protected readonly ArrayCollection Base;

		public BaseList(ArrayCollection obj)
		{
			this.Base = obj;
		}
	}
}