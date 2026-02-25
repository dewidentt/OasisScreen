using System;
using System.Windows.Forms;

namespace ScreenCaptureApp
{
	public class HotKeyEventArgs : EventArgs
	{
		public Keys Key { get; }
		public int Id { get; }

		public HotKeyEventArgs(Keys key, int id = 0)
		{
			this.Key = key;
			this.Id = id;
		}
	}
}
