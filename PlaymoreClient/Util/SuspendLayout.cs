using System;
using System.Windows.Forms;

namespace PlaymoreClient.Util
{
	public class SuspendLayout : IDisposable
	{
		protected readonly Control control;

		public SuspendLayout(Control cont)
		{
			this.control = cont;
			this.control.SuspendLayout();
		}

		public void Dispose()
		{
			this.control.ResumeLayout();
		}
	}
}