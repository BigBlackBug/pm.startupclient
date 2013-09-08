using System;

namespace PlaymoreClient.Messaging
{
	public interface IMessageProcessor
	{
		event CallHandler CallResult;

		event NotifyHandler Notify;

		event ProcessObjectHandler ProcessObject;
	}
}