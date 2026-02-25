using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace ScreenCaptureApp
{
	public partial class SettingsForm : Form
	{
		public Keys SelectedHotKey { get; private set; }
		public KeyModifiers SelectedModifiers { get; private set; }
		public Keys SelectedInstantHotKey { get; private set; }
		public KeyModifiers SelectedInstantModifiers { get; private set; }
		public Color BorderColor { get; private set; }

		public bool SaveToClipboard
		{
			get { return this.clipboardCheckBox.Checked; }
		}

		public bool RunAtStartup
		{
			get { return this.runAtStartupCheckBox.Checked; }
		}

		public string SavePath { get; private set; }

		public SettingsForm(Keys initialHotKey, KeyModifiers initialModifiers,
			Keys initialInstantHotKey, KeyModifiers initialInstantModifiers,
			bool initialClipboard, Color initialBorderColor, bool initialRunAtStartup,
			string initialSavePath)
		{
			this.InitializeComponent();
			this.SelectedHotKey = initialHotKey;
			this.SelectedModifiers = initialModifiers;
			this.SelectedInstantHotKey = initialInstantHotKey;
			this.SelectedInstantModifiers = initialInstantModifiers;
			this.clipboardCheckBox.Checked = initialClipboard;
			this.BorderColor = initialBorderColor;
			this.runAtStartupCheckBox.Checked = initialRunAtStartup;
			this.hotKeyTextBox.Text = FormatHotKey(initialHotKey, initialModifiers);
			this.instantHotKeyTextBox.Text = FormatHotKey(initialInstantHotKey, initialInstantModifiers);
			this.colorButton.BackColor = this.BorderColor;
			this.SavePath = initialSavePath ?? "";
			this.savePathTextBox.Text = this.SavePath;
		}

		private void HotKeyTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			e.SuppressKeyPress = true;
			this.SelectedHotKey = e.KeyCode;
			this.SelectedModifiers = KeyModifiers.None;
			if (e.Control) this.SelectedModifiers |= KeyModifiers.Control;
			if (e.Shift) this.SelectedModifiers |= KeyModifiers.Shift;
			if (e.Alt) this.SelectedModifiers |= KeyModifiers.Alt;
			this.hotKeyTextBox.Text = FormatHotKey(this.SelectedHotKey, this.SelectedModifiers);
		}

		private void InstantHotKeyTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			e.SuppressKeyPress = true;
			this.SelectedInstantHotKey = e.KeyCode;
			this.SelectedInstantModifiers = KeyModifiers.None;
			if (e.Control) this.SelectedInstantModifiers |= KeyModifiers.Control;
			if (e.Shift) this.SelectedInstantModifiers |= KeyModifiers.Shift;
			if (e.Alt) this.SelectedInstantModifiers |= KeyModifiers.Alt;
			this.instantHotKeyTextBox.Text = FormatHotKey(this.SelectedInstantHotKey, this.SelectedInstantModifiers);
		}

		private void ColorButton_Click(object sender, EventArgs e)
		{
			using (ColorDialog colorDialog = new ColorDialog())
			{
				colorDialog.Color = this.BorderColor;
				if (colorDialog.ShowDialog() == DialogResult.OK)
				{
					this.BorderColor = colorDialog.Color;
					this.colorButton.BackColor = this.BorderColor;
				}
			}
		}

		private void BrowseButton_Click(object sender, EventArgs e)
		{
			using (FolderBrowserDialog dlg = new FolderBrowserDialog())
			{
				dlg.SelectedPath = this.SavePath;
				dlg.Description = "Выберите папку для сохранения снимков";
				if (dlg.ShowDialog() == DialogResult.OK)
				{
					this.SavePath = dlg.SelectedPath;
					this.savePathTextBox.Text = dlg.SelectedPath;
				}
			}
		}

		private static string FormatHotKey(Keys key, KeyModifiers modifiers)
		{
			string str = "";
			if (modifiers.HasFlag(KeyModifiers.Control)) str += "Ctrl + ";
			if (modifiers.HasFlag(KeyModifiers.Shift)) str += "Shift + ";
			if (modifiers.HasFlag(KeyModifiers.Alt)) str += "Alt + ";
			return str + key.ToString();
		}
	}
}
