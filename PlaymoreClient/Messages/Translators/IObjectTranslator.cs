using FluorineFx;
using FluorineFx.AMF3;
using System;

namespace PlaymoreClient.Messages.Translators
{
	public interface IObjectTranslator
	{
		object GetArray(ArrayCollection array);

		object GetObject(ASObject flashobj);
	}
}