using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;

namespace ScreenCaptureApp
{
	public partial class MainForm : Form
	{
		private NotifyIcon trayIcon;
		private ContextMenuStrip trayMenu;
		private bool saveToClipboard = false;
		private Keys hotKey = Keys.S;
		private KeyModifiers hotKeyModifiers = KeyModifiers.Control | KeyModifiers.Shift;
		private Keys instantHotKey = Keys.S;
		private KeyModifiers instantHotKeyModifiers = KeyModifiers.Control | KeyModifiers.Alt;
		private Color borderColor = Color.Red;
		private bool runAtStartup = false;
		private int screenshotHotkeyId = -1;
		private int instantHotkeyId = -1;
		private string lastSavePath;

		public MainForm()
		{
			this.InitializeComponents();
			this.DoubleBuffered = true;
			this.LoadLastSavePath();
			this.LoadHotKeySettings();
			HotKeyManager.Initialize();
			HotKeyManager.HotKeyPressed += this.HotKeyManager_HotKeyPressed;
			this.RegisterHotKeys();
		}

		private void InitializeComponents()
		{
			base.FormBorderStyle = FormBorderStyle.None;
			base.ShowInTaskbar = false;
			base.Opacity = 0.0;

			this.trayMenu = new ContextMenuStrip();
			this.trayMenu.Renderer = new DarkMenuRenderer();
			this.trayMenu.Items.Add("\u2699  Настройки", null, new EventHandler(this.OnSettingsClick));
			this.trayMenu.Items.Add(new ToolStripSeparator());
			this.trayMenu.Items.Add("\u274C  Выход", null, new EventHandler(this.OnExitClick));

			this.trayIcon = new NotifyIcon
			{
				Icon = new Icon(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "oasis.ico")),
				ContextMenuStrip = this.trayMenu,
				Visible = true,
				Text = "OasisScreen"
			};
		}

		private void LoadLastSavePath()
		{
			using (var key = Registry.CurrentUser.OpenSubKey("Software\\ScreenCaptureApp", false))
			{
				if (key != null)
				{
					var val = key.GetValue("LastSavePath");
					this.lastSavePath = val?.ToString();
				}
			}
			if (string.IsNullOrEmpty(this.lastSavePath))
				this.lastSavePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Снимки экрана");
			if (!Directory.Exists(this.lastSavePath))
				Directory.CreateDirectory(this.lastSavePath);
		}

		private void SaveLastSavePath()
		{
			using (var key = Registry.CurrentUser.CreateSubKey("Software\\ScreenCaptureApp"))
			{
				key.SetValue("LastSavePath", this.lastSavePath);
			}
		}

		private void LoadHotKeySettings()
		{
			using (var key = Registry.CurrentUser.OpenSubKey("Software\\ScreenCaptureApp", false))
			{
				if (key != null)
				{
					this.hotKey = (Keys)Enum.Parse(typeof(Keys), key.GetValue("HotKey")?.ToString() ?? "S");
					this.hotKeyModifiers = (KeyModifiers)int.Parse(key.GetValue("HotKeyModifiers")?.ToString() ?? "6");
					this.saveToClipboard = bool.Parse(key.GetValue("SaveToClipboard")?.ToString() ?? "false");
					this.borderColor = Color.FromArgb(int.Parse(key.GetValue("BorderColor")?.ToString() ?? Color.Red.ToArgb().ToString()));
					this.runAtStartup = bool.Parse(key.GetValue("RunAtStartup")?.ToString() ?? "false");
					this.instantHotKey = (Keys)Enum.Parse(typeof(Keys), key.GetValue("InstantHotKey")?.ToString() ?? "S");
					this.instantHotKeyModifiers = (KeyModifiers)int.Parse(key.GetValue("InstantHotKeyModifiers")?.ToString() ?? "3");
				}
			}
			this.UpdateRunAtStartup();
		}

		private void SaveHotKeySettings()
		{
			using (var key = Registry.CurrentUser.CreateSubKey("Software\\ScreenCaptureApp"))
			{
				key.SetValue("HotKey", this.hotKey.ToString());
				key.SetValue("HotKeyModifiers", (int)this.hotKeyModifiers);
				key.SetValue("SaveToClipboard", this.saveToClipboard.ToString());
				key.SetValue("BorderColor", this.borderColor.ToArgb());
				key.SetValue("RunAtStartup", this.runAtStartup.ToString());
				key.SetValue("InstantHotKey", this.instantHotKey.ToString());
				key.SetValue("InstantHotKeyModifiers", (int)this.instantHotKeyModifiers);
			}
			this.UpdateRunAtStartup();
		}

		private void UpdateRunAtStartup()
		{
			using (var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
			{
				if (this.runAtStartup)
				key.SetValue("OasisScreen", Application.ExecutablePath);
				else
					key.DeleteValue("OasisScreen", false);
			}
		}

		private void RegisterHotKeys()
		{
			HotKeyManager.UnregisterHotKey();
			this.screenshotHotkeyId = HotKeyManager.RegisterHotKey(this.hotKey, this.hotKeyModifiers);
			this.instantHotkeyId = HotKeyManager.RegisterHotKey(this.instantHotKey, this.instantHotKeyModifiers);
		}

		private void HotKeyManager_HotKeyPressed(object sender, HotKeyEventArgs e)
		{
			if (e.Id == this.screenshotHotkeyId)
			{
				base.Invoke(new Action(this.OpenScreenshotEditor));
			}
			else if (e.Id == this.instantHotkeyId)
			{
				base.Invoke(new Action(this.TakeInstantScreenshot));
			}
		}

		private void OpenScreenshotEditor()
		{
			using (Bitmap bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height))
			{
				using (Graphics g = Graphics.FromImage(bitmap))
				{
					g.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
				}
				using (Bitmap darkened = new Bitmap(bitmap.Width, bitmap.Height))
				{
					using (Graphics g2 = Graphics.FromImage(darkened))
					{
						g2.DrawImage(bitmap, 0, 0);
						using (var brush = new SolidBrush(Color.FromArgb(180, 0, 0, 0)))
						{
							g2.FillRectangle(brush, 0, 0, darkened.Width, darkened.Height);
						}
					}
					using (var form = new ScreenshotForm(this.borderColor, bitmap, darkened, this.saveToClipboard))
					{
						form.ShowDialog();
					}
				}
			}
		}

		private void TakeInstantScreenshot()
		{
			using (Bitmap bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height))
			{
				using (Graphics g = Graphics.FromImage(bitmap))
				{
					g.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
				}

				string fileName = GetNextFileName(this.lastSavePath);
				string fullPath = Path.Combine(this.lastSavePath, fileName);
				bitmap.Save(fullPath, ImageFormat.Png);

				if (this.saveToClipboard)
				{
					Clipboard.SetImage(bitmap);
				}

				this.trayIcon.BalloonTipTitle = "OasisScreen";
				this.trayIcon.BalloonTipText = "\u0421\u043D\u0438\u043C\u043E\u043A \u0441\u043E\u0445\u0440\u0430\u043D\u0451\u043D: " + fileName;
				this.trayIcon.ShowBalloonTip(2000);
			}
		}

		private static string GetNextFileName(string folder)
		{
			string baseName = "Screenshot";
			string path = Path.Combine(folder, baseName + ".png");
			int num = 1;
			while (File.Exists(path))
			{
				path = Path.Combine(folder, string.Format("{0} ({1}).png", baseName, num));
				num++;
			}
			return Path.GetFileName(path);
		}

		private void OnSettingsClick(object sender, EventArgs e)
		{
			using (var settingsForm = new SettingsForm(
				this.hotKey, this.hotKeyModifiers,
				this.instantHotKey, this.instantHotKeyModifiers,
				this.saveToClipboard, this.borderColor, this.runAtStartup,
				this.lastSavePath))
			{
				settingsForm.ShowDialog();
				if (settingsForm.DialogResult == DialogResult.OK)
				{
					this.hotKey = settingsForm.SelectedHotKey;
					this.hotKeyModifiers = settingsForm.SelectedModifiers;
					this.instantHotKey = settingsForm.SelectedInstantHotKey;
					this.instantHotKeyModifiers = settingsForm.SelectedInstantModifiers;
					this.saveToClipboard = settingsForm.SaveToClipboard;
					this.borderColor = settingsForm.BorderColor;
					this.runAtStartup = settingsForm.RunAtStartup;
					this.lastSavePath = settingsForm.SavePath;
					this.SaveLastSavePath();
					this.SaveHotKeySettings();
					this.RegisterHotKeys();
				}
			}
		}

		private void OnExitClick(object sender, EventArgs e)
		{
			this.trayIcon.Visible = false;
			Application.Exit();
		}
	}

	internal class DarkMenuRenderer : ToolStripProfessionalRenderer
	{
		public DarkMenuRenderer() : base(new DarkMenuColors()) { }

		protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
		{
			e.TextColor = Color.FromArgb(220, 220, 220);
			base.OnRenderItemText(e);
		}
	}

	internal class DarkMenuColors : ProfessionalColorTable
	{
		private Color bg = Color.FromArgb(30, 30, 34);
		private Color bgSel = Color.FromArgb(62, 62, 66);
		private Color border = Color.FromArgb(55, 55, 60);

		public override Color MenuItemSelected => bgSel;
		public override Color MenuItemSelectedGradientBegin => bgSel;
		public override Color MenuItemSelectedGradientEnd => bgSel;
		public override Color MenuBorder => border;
		public override Color MenuItemBorder => border;
		public override Color ToolStripDropDownBackground => bg;
		public override Color ImageMarginGradientBegin => bg;
		public override Color ImageMarginGradientMiddle => bg;
		public override Color ImageMarginGradientEnd => bg;
		public override Color SeparatorDark => border;
		public override Color SeparatorLight => border;
	}
}
