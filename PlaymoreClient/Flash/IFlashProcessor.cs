using System;

namespace PlaymoreClient.Flash
{
	public interface IFlashProcessor
	{
		event ProcessLineD ProcessLine;

		event ProcessObjectD ProcessObject;
	}
}