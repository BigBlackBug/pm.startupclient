using PlaymoreClient.Messages;
using PlaymoreClient.Messages.Translators;
using PlaymoreClient.Messaging;
using FluorineFx;
using FluorineFx.AMF3;
using System;
using System.Threading;

namespace PlaymoreClient.Messages.Readers
{
	public class MessageReader : IObjectReader
	{
		private IMessageProcessor Flash;

		public MessageReader(IMessageProcessor flash)
		{
			this.Flash = flash;
			this.Flash.ProcessObject += new ProcessObjectHandler(this.Flash_ProcessObject);
		}

		private void Flash_ProcessObject(object sender, object flashobj, long timestamp)
		{
            PlaymoreClient.Gui.MainForm.LOGGER.Info("process object - ");
            if (flashobj != null)
            {
                PlaymoreClient.Gui.MainForm.LOGGER.Info(flashobj.ToString());
            }
            else
            {
                PlaymoreClient.Gui.MainForm.LOGGER.Info("flash object is null");
            }
			if (this.ObjectRead == null)
			{
				return;
			}
			object obj = null;
			if (flashobj is ASObject)
			{
                PlaymoreClient.Gui.MainForm.LOGGER.Info("is a ASObject");
				obj = MessageTranslator.Instance.GetObject((ASObject)flashobj);
			}
			else if (flashobj is ArrayCollection)
            {
                PlaymoreClient.Gui.MainForm.LOGGER.Info("is an ArrayCollection");
				obj = MessageTranslator.Instance.GetArray((ArrayCollection)flashobj);
			}
			if (obj != null)
			{
				if (obj is MessageObject)
				{
                    PlaymoreClient.Gui.MainForm.LOGGER.Info("transformed to messageobject");
					((MessageObject)obj).TimeStamp = timestamp;
				}
				this.ObjectRead(obj);
			}
		}

		public event ObjectReadD ObjectRead;
	}
}