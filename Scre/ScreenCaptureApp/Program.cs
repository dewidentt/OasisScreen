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
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
			Application.ThreadException += (s, e) =>
			{
				MessageBox.Show("Ошибка: " + e.Exception.Message + "\n" + e.Exception.StackTrace,
					"OasisScreen — Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
			};
			AppDomain.CurrentDomain.UnhandledException += (s, e) =>
			{
				var ex = e.ExceptionObject as Exception;
				MessageBox.Show("Критическая ошибка: " + (ex?.Message ?? e.ExceptionObject.ToString()),
					"OasisScreen — Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
			};

			bool createdNew;
			mutex = new Mutex(true, "ScreenCaptureApp_SingleInstance", out createdNew);
			if (!createdNew)
			{
				MessageBox.Show("Приложение уже запущено.", "OasisScreen", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			Application.Run(new MainForm());
		}
	}
}
