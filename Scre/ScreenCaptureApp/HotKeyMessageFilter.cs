using System;
using System.Windows.Forms;

namespace ScreenCaptureApp
{
	public class HotKeyMessageFilter : IMessageFilter
	{
		private const int WM_HOTKEY = 786;

		public bool PreFilterMessage(ref Message m)
		{
			if (m.Msg == WM_HOTKEY)
			{
				int hotkeyId = m.WParam.ToInt32();
				Keys key = (Keys)((int)m.LParam >> 16 & 65535);
				HotKeyManager.OnHotKeyPressed(new HotKeyEventArgs(key, hotkeyId));
				return true;
			}
			return false;
		}
	}
}
