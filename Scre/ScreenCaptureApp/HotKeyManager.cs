using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ScreenCaptureApp
{
	public static class HotKeyManager
	{
		public static event EventHandler<HotKeyEventArgs> HotKeyPressed;

		private static Form messageForm;

		public static void Initialize()
		{
			messageForm = new Form();
			hWnd = messageForm.Handle;
			Application.AddMessageFilter(new HotKeyMessageFilter());
		}

		public static int RegisterHotKey(Keys key, KeyModifiers modifiers)
		{
			id++;
			bool success = RegisterHotKey(hWnd, id, (uint)modifiers, (uint)key);
			if (!success)
			{
				MessageBox.Show(
					"Не удалось зарегистрировать горячую клавишу.\nВозможно, она уже используется другим приложением.",
					"OasisScreen", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return -1;
			}
			registeredIds.Add(id);
			return id;
		}

		public static void UnregisterHotKey()
		{
			foreach (int regId in registeredIds)
			{
				UnregisterHotKey(hWnd, regId);
			}
			registeredIds.Clear();
		}

		public static void OnHotKeyPressed(HotKeyEventArgs e)
		{
			HotKeyPressed?.Invoke(null, e);
		}

		[DllImport("user32.dll")]
		private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

		[DllImport("user32.dll")]
		private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

		private const int WM_HOTKEY = 786;

		private static int id;

		private static IntPtr hWnd;

		private static List<int> registeredIds = new List<int>();
	}
}
