namespace ScreenCaptureApp
{
	public partial class SettingsForm : global::System.Windows.Forms.Form
	{
		private void InitializeComponent()
		{
			var bgDark = global::System.Drawing.Color.FromArgb(30, 30, 34);
			var bgCard = global::System.Drawing.Color.FromArgb(45, 45, 48);
			var bgInput = global::System.Drawing.Color.FromArgb(55, 55, 60);
			var fgMain = global::System.Drawing.Color.FromArgb(220, 220, 220);
			var fgDim = global::System.Drawing.Color.FromArgb(140, 140, 145);
			var accent = global::System.Drawing.Color.FromArgb(0, 122, 204);
			var accentHover = global::System.Drawing.Color.FromArgb(28, 151, 234);
			var fontMain = new global::System.Drawing.Font("Segoe UI", 9.5f);
			var fontTitle = new global::System.Drawing.Font("Segoe UI Semibold", 13f);
			var fontSmall = new global::System.Drawing.Font("Segoe UI", 8f);

			this.Text = "\u041D\u0430\u0441\u0442\u0440\u043E\u0439\u043A\u0438";
			base.Size = new global::System.Drawing.Size(400, 540);
			base.FormBorderStyle = global::System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.StartPosition = global::System.Windows.Forms.FormStartPosition.CenterScreen;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			this.BackColor = bgDark;
			this.ForeColor = fgMain;
			this.Font = fontMain;

			int left = 24;
			int w = 336;
			int y = 16;

			var titleLabel = new global::System.Windows.Forms.Label
			{
				Text = "\u2699  \u041D\u0430\u0441\u0442\u0440\u043E\u0439\u043A\u0438",
				Font = fontTitle,
				ForeColor = fgMain,
				Location = new global::System.Drawing.Point(left, y),
				AutoSize = true
			};
			y += 32;

			var separator = new global::System.Windows.Forms.Label
			{
				BorderStyle = global::System.Windows.Forms.BorderStyle.Fixed3D,
				Location = new global::System.Drawing.Point(left, y),
				Size = new global::System.Drawing.Size(w, 2),
				BackColor = bgInput
			};
			y += 14;

			var hotkeyLabel = new global::System.Windows.Forms.Label
			{
				Text = "\u0413\u043E\u0440\u044F\u0447\u0430\u044F \u043A\u043B\u0430\u0432\u0438\u0448\u0430 (\u0441\u043D\u0438\u043C\u043E\u043A \u0441 \u0432\u044B\u0431\u043E\u0440\u043E\u043C)",
				ForeColor = fgDim,
				Font = fontMain,
				Location = new global::System.Drawing.Point(left, y),
				AutoSize = true
			};
			y += 22;

			this.hotKeyTextBox = new global::System.Windows.Forms.TextBox
			{
				Location = new global::System.Drawing.Point(left, y),
				Size = new global::System.Drawing.Size(w, 28),
				ReadOnly = true,
				BackColor = bgInput,
				ForeColor = fgMain,
				Font = fontMain,
				BorderStyle = global::System.Windows.Forms.BorderStyle.FixedSingle
			};
			this.hotKeyTextBox.KeyDown += new global::System.Windows.Forms.KeyEventHandler(this.HotKeyTextBox_KeyDown);
			y += 38;

			var instantLabel = new global::System.Windows.Forms.Label
			{
				Text = "\u0413\u043E\u0440\u044F\u0447\u0430\u044F \u043A\u043B\u0430\u0432\u0438\u0448\u0430 (\u043C\u043E\u043C\u0435\u043D\u0442\u0430\u043B\u044C\u043D\u044B\u0439 \u0441\u043D\u0438\u043C\u043E\u043A)",
				ForeColor = fgDim,
				Font = fontMain,
				Location = new global::System.Drawing.Point(left, y),
				AutoSize = true
			};
			y += 22;

			this.instantHotKeyTextBox = new global::System.Windows.Forms.TextBox
			{
				Location = new global::System.Drawing.Point(left, y),
				Size = new global::System.Drawing.Size(w, 28),
				ReadOnly = true,
				BackColor = bgInput,
				ForeColor = fgMain,
				Font = fontMain,
				BorderStyle = global::System.Windows.Forms.BorderStyle.FixedSingle
			};
			this.instantHotKeyTextBox.KeyDown += new global::System.Windows.Forms.KeyEventHandler(this.InstantHotKeyTextBox_KeyDown);
			y += 38;

			var borderLabel = new global::System.Windows.Forms.Label
			{
				Text = "\u0426\u0432\u0435\u0442 \u0440\u0430\u043C\u043A\u0438",
				ForeColor = fgDim,
				Font = fontMain,
				Location = new global::System.Drawing.Point(left, y),
				AutoSize = true
			};
			y += 22;

			this.colorButton = new global::System.Windows.Forms.Button
			{
				Text = "",
				Location = new global::System.Drawing.Point(left, y),
				Size = new global::System.Drawing.Size(w, 32),
				FlatStyle = global::System.Windows.Forms.FlatStyle.Flat,
				Cursor = global::System.Windows.Forms.Cursors.Hand
			};
			this.colorButton.FlatAppearance.BorderSize = 1;
			this.colorButton.FlatAppearance.BorderColor = bgInput;
			this.colorButton.Click += new global::System.EventHandler(this.ColorButton_Click);
			y += 42;

			this.clipboardCheckBox = new global::System.Windows.Forms.CheckBox
			{
				Text = "  \u041A\u043E\u043F\u0438\u0440\u043E\u0432\u0430\u0442\u044C \u0432 \u0431\u0443\u0444\u0435\u0440 \u043F\u043E\u0441\u043B\u0435 \u0441\u043E\u0445\u0440\u0430\u043D\u0435\u043D\u0438\u044F",
				ForeColor = fgMain,
				Font = fontMain,
				Location = new global::System.Drawing.Point(left - 2, y),
				AutoSize = true,
				FlatStyle = global::System.Windows.Forms.FlatStyle.Flat
			};
			y += 30;

			this.runAtStartupCheckBox = new global::System.Windows.Forms.CheckBox
			{
				Text = "  \u0417\u0430\u043F\u0443\u0441\u043A\u0430\u0442\u044C \u043F\u0440\u0438 \u0441\u0442\u0430\u0440\u0442\u0435 Windows",
				ForeColor = fgMain,
				Font = fontMain,
				Location = new global::System.Drawing.Point(left - 2, y),
				AutoSize = true,
				FlatStyle = global::System.Windows.Forms.FlatStyle.Flat
			};
			y += 30;

			var savePathLabel = new global::System.Windows.Forms.Label
			{
				Text = "\u041F\u0430\u043F\u043A\u0430 \u0441\u043E\u0445\u0440\u0430\u043D\u0435\u043D\u0438\u044F",
				ForeColor = fgDim,
				Font = fontMain,
				Location = new global::System.Drawing.Point(left, y),
				AutoSize = true
			};
			y += 22;

			this.savePathTextBox = new global::System.Windows.Forms.TextBox
			{
				Location = new global::System.Drawing.Point(left, y),
				Size = new global::System.Drawing.Size(w - 38, 28),
				ReadOnly = true,
				BackColor = bgInput,
				ForeColor = fgMain,
				Font = fontMain,
				BorderStyle = global::System.Windows.Forms.BorderStyle.FixedSingle
			};

			this.browseButton = new global::System.Windows.Forms.Button
			{
				Text = "...",
				Location = new global::System.Drawing.Point(left + w - 34, y),
				Size = new global::System.Drawing.Size(34, 28),
				FlatStyle = global::System.Windows.Forms.FlatStyle.Flat,
				BackColor = bgCard,
				ForeColor = fgMain,
				Font = fontMain,
				Cursor = global::System.Windows.Forms.Cursors.Hand
			};
			this.browseButton.FlatAppearance.BorderSize = 1;
			this.browseButton.FlatAppearance.BorderColor = bgInput;
			this.browseButton.Click += new global::System.EventHandler(this.BrowseButton_Click);
			y += 38;

			var okButton = new global::System.Windows.Forms.Button
			{
				Text = "\u0421\u043E\u0445\u0440\u0430\u043D\u0438\u0442\u044C",
				Size = new global::System.Drawing.Size(110, 36),
				Location = new global::System.Drawing.Point(left + w - 222, y),
				FlatStyle = global::System.Windows.Forms.FlatStyle.Flat,
				BackColor = accent,
				ForeColor = global::System.Drawing.Color.White,
				Font = fontMain,
				Cursor = global::System.Windows.Forms.Cursors.Hand,
				DialogResult = global::System.Windows.Forms.DialogResult.OK
			};
			okButton.FlatAppearance.BorderSize = 0;
			okButton.FlatAppearance.MouseOverBackColor = accentHover;

			var cancelButton = new global::System.Windows.Forms.Button
			{
				Text = "\u041E\u0442\u043C\u0435\u043D\u0430",
				Size = new global::System.Drawing.Size(100, 36),
				Location = new global::System.Drawing.Point(left + w - 100, y),
				FlatStyle = global::System.Windows.Forms.FlatStyle.Flat,
				BackColor = bgCard,
				ForeColor = fgDim,
				Font = fontMain,
				Cursor = global::System.Windows.Forms.Cursors.Hand,
				DialogResult = global::System.Windows.Forms.DialogResult.Cancel
			};
			cancelButton.FlatAppearance.BorderSize = 1;
			cancelButton.FlatAppearance.BorderColor = bgInput;
			cancelButton.FlatAppearance.MouseOverBackColor = bgInput;

			var linkLabel = new global::System.Windows.Forms.LinkLabel
			{
				Text = "t.me/dewimods",
				Font = fontSmall,
				LinkColor = fgDim,
				ActiveLinkColor = accent,
				VisitedLinkColor = fgDim,
				Location = new global::System.Drawing.Point(left, y + 8),
				AutoSize = true
			};
			linkLabel.LinkClicked += delegate(object s, global::System.Windows.Forms.LinkLabelLinkClickedEventArgs ev)
			{
				global::System.Diagnostics.Process.Start(new global::System.Diagnostics.ProcessStartInfo
				{
					FileName = "https://t.me/dewimods",
					UseShellExecute = true
				});
			};

			base.Controls.Add(titleLabel);
			base.Controls.Add(separator);
			base.Controls.Add(hotkeyLabel);
			base.Controls.Add(this.hotKeyTextBox);
			base.Controls.Add(instantLabel);
			base.Controls.Add(this.instantHotKeyTextBox);
			base.Controls.Add(borderLabel);
			base.Controls.Add(this.colorButton);
			base.Controls.Add(this.clipboardCheckBox);
			base.Controls.Add(this.runAtStartupCheckBox);
			base.Controls.Add(savePathLabel);
			base.Controls.Add(this.savePathTextBox);
			base.Controls.Add(this.browseButton);
			base.Controls.Add(okButton);
			base.Controls.Add(cancelButton);
			base.Controls.Add(linkLabel);

			okButton.Click += delegate(object s, global::System.EventArgs ev) { base.Close(); };
			cancelButton.Click += delegate(object s, global::System.EventArgs ev) { base.Close(); };
		}

		private global::System.Windows.Forms.CheckBox clipboardCheckBox;
		private global::System.Windows.Forms.CheckBox runAtStartupCheckBox;
		private global::System.Windows.Forms.TextBox hotKeyTextBox;
		private global::System.Windows.Forms.TextBox instantHotKeyTextBox;
		private global::System.Windows.Forms.Button colorButton;
		private global::System.Windows.Forms.TextBox savePathTextBox;
		private global::System.Windows.Forms.Button browseButton;
	}
}
