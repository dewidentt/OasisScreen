using System;
using System.Threading;
using System.Windows.Forms;

namespace ScreenCaptureApp
{
	internal static class Program
	{
		private static Mutex mutex;

		[STAThread]
		private static void Main()
		{
			bool createdNew;
			mutex = new Mutex(true, "ScreenCaptureApp_SingleInstance", out createdNew);
			if (!createdNew)
			{
				MessageBox.Show("Приложение уже запущено.", "OasisScreen", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}
	}
}
