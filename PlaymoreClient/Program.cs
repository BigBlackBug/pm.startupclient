using PlaymoreClient.Gui;
using System;
using System.Threading;
using System.Windows.Forms;

namespace PlaymoreClient
{
	internal static class Program
	{
		[STAThread]
		private static void Main()
		{
			bool flag = false;
			using (Mutex mutex = new Mutex(true, "ElophantClient", out flag))
			{
				if (!flag)
				{
					MessageBox.Show("The Elophant Client is already running.");
				}
				else
				{
					Application.EnableVisualStyles();
					Application.SetCompatibleTextRenderingDefault(false);
					Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
					Application.Run(new PMMainForm());
				}
			}
		}
	}
}