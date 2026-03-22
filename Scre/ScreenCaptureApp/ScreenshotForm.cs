using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace ScreenCaptureApp
{
	public partial class ScreenshotForm : Form
	{
		public ScreenshotForm(Color borderColorFromSettings, Bitmap originalScreenshot, Bitmap darkenedScreenshot, bool saveToClipboard = false)
		{
			this.DoubleBuffered = true;
			base.FormBorderStyle = FormBorderStyle.None;
			base.WindowState = FormWindowState.Maximized;
			base.TopMost = true;
			base.KeyPreview = true;
			this.BackColor = Color.Black;
			base.Opacity = 1.0;
			this.borderColor = borderColorFromSettings;
			this.saveToClipboard = saveToClipboard;
			this.originalScreenshot = originalScreenshot;
			this.darkenedScreenshot = darkenedScreenshot;
			this.LoadLastSavePath();
			this.InitializeButtons();
			this.ResetDrawingState();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.drawingBitmap != null) { this.drawingBitmap.Dispose(); this.drawingBitmap = null; }
				if (this.tempBitmap != null) { this.tempBitmap.Dispose(); this.tempBitmap = null; }
				if (this.originalScreenshot != null) { this.originalScreenshot.Dispose(); this.originalScreenshot = null; }
				if (this.darkenedScreenshot != null) { this.darkenedScreenshot.Dispose(); this.darkenedScreenshot = null; }
				if (this.shapesMenu != null) { this.shapesMenu.Dispose(); this.shapesMenu = null; }
				if (this.toolTip != null) { this.toolTip.Dispose(); this.toolTip = null; }
				this.ClearUndoRedoStacks();
			}
			base.Dispose(disposing);
		}

		private void LoadLastSavePath()
		{
			using (RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\ScreenCaptureApp", false))
			{
				string val = null;
				if (key != null)
				{
					object v = key.GetValue("LastSavePath");
					if (v != null) val = v.ToString();
				}
				string defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Снимки экрана");
				this.lastSavePath = val ?? defaultPath;
			}
			if (!Directory.Exists(this.lastSavePath))
				Directory.CreateDirectory(this.lastSavePath);
		}

		private void SaveLastSavePath()
		{
			using (RegistryKey key = Registry.CurrentUser.CreateSubKey("Software\\ScreenCaptureApp"))
			{
				key.SetValue("LastSavePath", this.lastSavePath);
			}
		}

		private void InitializeButtons()
		{
			this.toolTip = new ToolTip
			{
				BackColor = Color.FromArgb(45, 45, 48),
				ForeColor = Color.FromArgb(220, 220, 220),
				OwnerDraw = false,
				InitialDelay = 400,
				ReshowDelay = 200
			};

			this.saveButton = this.CreateToolButton("\ud83d\udcbe");
			this.saveButton.Click += this.SaveButton_Click;

			this.drawButton = this.CreateToolButton("\u270f\ufe0f");
			this.drawButton.Click += this.DrawButton_Click;

			this.textButton = this.CreateToolButton("\ud83c\udd70");
			this.textButton.Click += this.TextButton_Click;

			this.arrowButton = this.CreateToolButton("\u2197");
			this.arrowButton.Click += this.ArrowButton_Click;

			this.colorButton = this.CreateToolButton("\ud83c\udfa8");
			this.colorButton.Click += this.ColorButton_Click;

			this.magnifierButton = this.CreateToolButton("\ud83d\udd0d");
			this.magnifierButton.Click += this.MagnifierButton_Click;

			this.ocrButton = this.CreateToolButton("\ud83d\udd24");
			this.ocrButton.Click += this.OcrButton_Click;

			this.mosaicButton = this.CreateToolButton("\ud83d\udfe5");
			this.mosaicButton.Click += this.MosaicButton_Click;

			this.shapesButton = this.CreateToolButton("\u2605");
			this.shapesMenu = new ContextMenuStrip();
			this.shapesMenu.Renderer = new DarkShapesMenuRenderer();
			this.shapesMenu.Items.Add("\u041f\u0440\u044f\u043c\u043e\u0443\u0433\u043e\u043b\u044c\u043d\u0438\u043a", null, delegate(object s, EventArgs ev)
			{
				this.currentTool = Tool.Quad;
				this.quadPointCount = 0;
				this.UpdateCursor();
			});
			this.shapesMenu.Items.Add("\u041a\u0440\u0443\u0433", null, delegate(object s, EventArgs ev)
			{
				this.currentTool = Tool.Circle;
				this.UpdateCursor();
			});
			this.shapesMenu.Items.Add("\u041f\u043e\u0434\u0447\u0451\u0440\u043a\u0438\u0432\u0430\u043d\u0438\u0435", null, delegate(object s, EventArgs ev)
			{
				this.currentTool = Tool.Underline;
				this.UpdateCursor();
			});
			this.shapesButton.Click += delegate(object s, EventArgs ev)
			{
				this.shapesMenu.Show(this.shapesButton, new Point(0, this.shapesButton.Height));
			};

			this.closeButton = this.CreateToolButton("\u2716");
			this.closeButton.Click += this.CloseButton_Click;

			this.toolTip.SetToolTip(this.closeButton, "\u0417\u0430\u043a\u0440\u044b\u0442\u044c");
			this.toolTip.SetToolTip(this.shapesButton, "\u0424\u0438\u0433\u0443\u0440\u044b");
			this.toolTip.SetToolTip(this.mosaicButton, "\u041c\u043e\u0437\u0430\u0438\u043a\u0430");
			this.toolTip.SetToolTip(this.drawButton, "\u041a\u0430\u0440\u0430\u043d\u0434\u0430\u0448");
			this.toolTip.SetToolTip(this.arrowButton, "\u0421\u0442\u0440\u0435\u043b\u043a\u0430");
			this.toolTip.SetToolTip(this.textButton, "\u0422\u0435\u043a\u0441\u0442");
			this.toolTip.SetToolTip(this.colorButton, "\u0426\u0432\u0435\u0442");
			this.toolTip.SetToolTip(this.saveButton, "\u0421\u043e\u0445\u0440\u0430\u043d\u0438\u0442\u044c");
			this.toolTip.SetToolTip(this.magnifierButton, "\u041b\u0443\u043f\u0430");
			this.toolTip.SetToolTip(this.ocrButton, "\u0420\u0430\u0441\u043f\u043e\u0437\u043d\u0430\u0442\u044c \u0442\u0435\u043a\u0441\u0442");

			int btnW = 40;
			int btnGap = 3;
			int pad = 6;
			int col = 0;
			this.closeButton.Location = new Point(pad + col * (btnW + btnGap), pad); col++;
			this.shapesButton.Location = new Point(pad + col * (btnW + btnGap), pad); col++;
			this.mosaicButton.Location = new Point(pad + col * (btnW + btnGap), pad); col++;
			this.drawButton.Location = new Point(pad + col * (btnW + btnGap), pad); col++;
			this.arrowButton.Location = new Point(pad + col * (btnW + btnGap), pad); col++;
			this.textButton.Location = new Point(pad + col * (btnW + btnGap), pad); col++;
			this.colorButton.Location = new Point(pad + col * (btnW + btnGap), pad); col++;
			this.magnifierButton.Location = new Point(pad + col * (btnW + btnGap), pad); col++;
			this.ocrButton.Location = new Point(pad + col * (btnW + btnGap), pad); col++;
			this.saveButton.Location = new Point(pad + col * (btnW + btnGap), pad); col++;

			int totalBtns = col;
			int panelW = pad * 2 + totalBtns * btnW + (totalBtns - 1) * btnGap;
			int panelH = pad * 2 + btnW;

			this.toolbarPanel = new Panel
			{
				BackColor = Color.FromArgb(45, 45, 48),
				Visible = false,
				Size = new Size(panelW, panelH)
			};

			int radius = 10;
			GraphicsPath path = new GraphicsPath();
			path.AddArc(0, 0, radius, radius, 180, 90);
			path.AddArc(panelW - radius, 0, radius, radius, 270, 90);
			path.AddArc(panelW - radius, panelH - radius, radius, radius, 0, 90);
			path.AddArc(0, panelH - radius, radius, radius, 90, 90);
			path.CloseFigure();
			this.toolbarPanel.Region = new Region(path);

			this.toolbarPanel.Controls.Add(this.closeButton);
			this.toolbarPanel.Controls.Add(this.shapesButton);
			this.toolbarPanel.Controls.Add(this.mosaicButton);
			this.toolbarPanel.Controls.Add(this.drawButton);
			this.toolbarPanel.Controls.Add(this.arrowButton);
			this.toolbarPanel.Controls.Add(this.textButton);
			this.toolbarPanel.Controls.Add(this.colorButton);
			this.toolbarPanel.Controls.Add(this.magnifierButton);
			this.toolbarPanel.Controls.Add(this.ocrButton);
			this.toolbarPanel.Controls.Add(this.saveButton);
			base.Controls.Add(this.toolbarPanel);
		}

		private Button CreateToolButton(string text)
		{
			var btn = new Button
			{
				Text = text,
				Size = new Size(40, 40),
				FlatStyle = FlatStyle.Flat,
				BackColor = Color.FromArgb(62, 62, 66),
				ForeColor = Color.White,
				Font = new Font("Segoe UI Emoji", 11f),
				Cursor = Cursors.Hand
			};
			btn.FlatAppearance.BorderSize = 0;
			btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(80, 80, 85);
			btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(100, 100, 108);
			return btn;
		}

		private void UpdateCursor()
		{
			this.Cursor = (this.currentTool == Tool.None) ? Cursors.Default : Cursors.Cross;
			Color normalColor = Color.FromArgb(62, 62, 66);
			Color activeColor = Color.FromArgb(0, 122, 204);
			this.drawButton.BackColor = (this.currentTool == Tool.Pencil) ? activeColor : normalColor;
			this.mosaicButton.BackColor = (this.currentTool == Tool.Mosaic) ? activeColor : normalColor;
			this.arrowButton.BackColor = (this.currentTool == Tool.Arrow) ? activeColor : normalColor;
			this.textButton.BackColor = (this.currentTool == Tool.Text) ? activeColor : normalColor;
			this.magnifierButton.BackColor = (this.currentTool == Tool.Magnifier) ? activeColor : normalColor;
			bool shapesActive = this.currentTool == Tool.Quad || this.currentTool == Tool.Circle || this.currentTool == Tool.Underline;
			this.shapesButton.BackColor = shapesActive ? activeColor : normalColor;
		}

		private void ResetDrawingState()
		{
			this.selectionRect = Rectangle.Empty;
			if (this.drawingBitmap != null) { this.drawingBitmap.Dispose(); this.drawingBitmap = null; }
			if (this.tempBitmap != null) { this.tempBitmap.Dispose(); this.tempBitmap = null; }
			this.lastPoint = Point.Empty;
			this.currentTool = Tool.None;
			this.quadPointCount = 0;
			this.ClearUndoRedoStacks();
			this.toolbarPanel.Visible = false;
			this.Cursor = Cursors.Default;
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
			{
				e.Handled = true;
				this.CommitTextOverlay();
				base.Close();
				return;
			}
			if (e.Control && e.KeyCode == Keys.Z)
			{
				e.Handled = true;
				this.Undo();
				return;
			}
			if (e.Control && e.KeyCode == Keys.Y)
			{
				e.Handled = true;
				this.Redo();
				return;
			}
			base.OnKeyDown(e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				if (this.currentTool == Tool.Text)
				{
					this.CommitTextOverlay();
					if (this.drawingBitmap != null)
					{
						Point local = new Point(e.X - this.selectionRect.X, e.Y - this.selectionRect.Y);
						this.ShowTextOverlay(local);
					}
					base.OnMouseDown(e);
					return;
				}

				if (this.currentTool == Tool.None)
				{
					if (this.drawingBitmap != null)
					{
						this.drawingBitmap.Dispose();
						this.drawingBitmap = null;
						this.ResetDrawingState();
					}
					this.isSelecting = true;
					this.startPoint = e.Location;
					this.selectionRect = new Rectangle(e.Location, new Size(0, 0));
				}
				else if (this.drawingBitmap != null && this.currentTool > Tool.None)
				{
					this.lastPoint = new Point(e.X - this.selectionRect.X, e.Y - this.selectionRect.Y);
					if (this.currentTool == Tool.Quad)
					{
						if (this.quadPointCount < 4)
						{
							this.quadPoints[this.quadPointCount] = this.lastPoint;
							this.quadPointCount++;
							if (this.quadPointCount == 4)
							{
								this.SaveUndoState();
								using (Graphics g = Graphics.FromImage(this.drawingBitmap))
								{
									g.SmoothingMode = SmoothingMode.AntiAlias;
									using (Pen pen = new Pen(this.penColor, 3f))
									{
										g.DrawPolygon(pen, this.quadPoints);
									}
								}
								this.quadPointCount = 0;
								this.currentTool = Tool.None;
								this.UpdateCursor();
							}
						}
					}
					else
					{
						this.SaveUndoState();
					}
				}
			}
			base.OnMouseDown(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (this.isSelecting)
			{
				int dx = e.X - this.startPoint.X;
				int dy = e.Y - this.startPoint.Y;
				this.selectionRect = new Rectangle(
					Math.Min(e.X, this.startPoint.X),
					Math.Min(e.Y, this.startPoint.Y),
					Math.Abs(dx), Math.Abs(dy));
				base.Invalidate();
			}
			else if (this.drawingBitmap != null && this.lastPoint != Point.Empty)
			{
				Point pt = new Point(e.X - this.selectionRect.X, e.Y - this.selectionRect.Y);
				if (this.tempBitmap != null) this.tempBitmap.Dispose();
				this.tempBitmap = new Bitmap(this.drawingBitmap);

				using (Graphics g = Graphics.FromImage(this.tempBitmap))
				{
					g.SmoothingMode = SmoothingMode.AntiAlias;
					using (Pen pen = new Pen(this.penColor, 3f))
					{
						if (this.currentTool == Tool.Pencil)
						{
							g.DrawLine(pen, this.lastPoint, pt);
							using (Graphics gd = Graphics.FromImage(this.drawingBitmap))
							{
								gd.SmoothingMode = SmoothingMode.AntiAlias;
								gd.DrawLine(pen, this.lastPoint, pt);
							}
							this.lastPoint = pt;
						}
						else if (this.currentTool == Tool.Mosaic)
						{
							this.ApplyMosaicEffect(g, pt);
							using (Graphics gd = Graphics.FromImage(this.drawingBitmap))
							{
								this.ApplyMosaicEffect(gd, pt);
							}
							this.lastPoint = pt;
						}
						else if (this.currentTool == Tool.Circle)
						{
							int r = (int)Math.Sqrt(Math.Pow(pt.X - this.lastPoint.X, 2) + Math.Pow(pt.Y - this.lastPoint.Y, 2));
							g.DrawEllipse(pen, this.lastPoint.X - r, this.lastPoint.Y - r, r * 2, r * 2);
						}
						else if (this.currentTool == Tool.Underline)
						{
							g.DrawLine(pen, this.lastPoint, pt);
						}
						else if (this.currentTool == Tool.Arrow)
						{
							DrawArrow(g, pen, this.lastPoint, pt);
						}
						else if (this.currentTool == Tool.Quad && this.quadPointCount > 0)
						{
							Point[] arr = new Point[this.quadPointCount + 1];
							Array.Copy(this.quadPoints, 0, arr, 0, this.quadPointCount);
							arr[this.quadPointCount] = pt;
							g.DrawLines(pen, arr);
						}
					}
					if (this.currentTool == Tool.Magnifier)
					{
						int radius = (int)Math.Sqrt(Math.Pow(pt.X - this.lastPoint.X, 2) + Math.Pow(pt.Y - this.lastPoint.Y, 2));
						this.DrawMagnifier(g, this.drawingBitmap, this.lastPoint, radius);
					}
				}
				base.Invalidate();
			}
			base.OnMouseMove(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (this.isSelecting)
			{
				this.isSelecting = false;
				if (this.selectionRect.Width > 10 && this.selectionRect.Height > 10)
				{
					this.drawingBitmap = new Bitmap(this.selectionRect.Width, this.selectionRect.Height);
					using (Graphics g = Graphics.FromImage(this.drawingBitmap))
					{
						g.DrawImage(this.originalScreenshot, 0, 0, this.selectionRect, GraphicsUnit.Pixel);
					}
					this.PositionButtons();
				}
			}
			else if (this.drawingBitmap != null)
			{
				if (this.currentTool == Tool.Circle || this.currentTool == Tool.Underline || this.currentTool == Tool.Arrow)
				{
					Point pt = new Point(e.X - this.selectionRect.X, e.Y - this.selectionRect.Y);
					using (Graphics g = Graphics.FromImage(this.drawingBitmap))
					{
						g.SmoothingMode = SmoothingMode.AntiAlias;
						using (Pen pen = new Pen(this.penColor, 3f))
						{
							if (this.currentTool == Tool.Circle)
							{
								int r = (int)Math.Sqrt(Math.Pow(pt.X - this.lastPoint.X, 2) + Math.Pow(pt.Y - this.lastPoint.Y, 2));
								g.DrawEllipse(pen, this.lastPoint.X - r, this.lastPoint.Y - r, r * 2, r * 2);
							}
							else if (this.currentTool == Tool.Underline)
							{
								g.DrawLine(pen, this.lastPoint, pt);
							}
							else if (this.currentTool == Tool.Arrow)
							{
								DrawArrow(g, pen, this.lastPoint, pt);
							}
						}
					}
					this.currentTool = Tool.None;
					this.UpdateCursor();
					base.Invalidate();
				}
				else if (this.currentTool == Tool.Magnifier)
				{
					Point pt = new Point(e.X - this.selectionRect.X, e.Y - this.selectionRect.Y);
					int radius = (int)Math.Sqrt(Math.Pow(pt.X - this.lastPoint.X, 2) + Math.Pow(pt.Y - this.lastPoint.Y, 2));
					if (radius >= 15)
					{
						using (Bitmap source = new Bitmap(this.drawingBitmap))
						using (Graphics g = Graphics.FromImage(this.drawingBitmap))
						{
							this.DrawMagnifier(g, source, this.lastPoint, radius);
						}
					}
					this.currentTool = Tool.None;
					this.UpdateCursor();
					base.Invalidate();
				}
			}
			if (this.currentTool != Tool.Quad)
			{
				this.lastPoint = Point.Empty;
			}
			if (this.tempBitmap != null) { this.tempBitmap.Dispose(); this.tempBitmap = null; }
			base.OnMouseUp(e);
		}

		private static void DrawArrow(Graphics g, Pen pen, Point from, Point to)
		{
			g.DrawLine(pen, from, to);
			double angle = Math.Atan2(to.Y - from.Y, to.X - from.X);
			int headLen = 14;
			double spread = Math.PI / 7;
			PointF p1 = new PointF(
				(float)(to.X - headLen * Math.Cos(angle - spread)),
				(float)(to.Y - headLen * Math.Sin(angle - spread)));
			PointF p2 = new PointF(
				(float)(to.X - headLen * Math.Cos(angle + spread)),
				(float)(to.Y - headLen * Math.Sin(angle + spread)));
			using (SolidBrush brush = new SolidBrush(pen.Color))
			{
				g.FillPolygon(brush, new PointF[] { to, p1, p2 });
			}
		}

		private void ApplyMosaicEffect(Graphics g, Point point)
		{
			int sz = 10;
			Rectangle area = new Rectangle(point.X - sz / 2, point.Y - sz / 2, sz, sz);
			area = Rectangle.Intersect(area, new Rectangle(0, 0, this.drawingBitmap.Width, this.drawingBitmap.Height));
			if (area.Width > 0 && area.Height > 0)
			{
				using (Bitmap chunk = new Bitmap(area.Width, area.Height))
				{
					using (Graphics cg = Graphics.FromImage(chunk))
					{
						cg.DrawImage(this.drawingBitmap, 0, 0, area, GraphicsUnit.Pixel);
					}
					using (Bitmap small = new Bitmap(chunk, new Size(Math.Max(1, area.Width / 2), Math.Max(1, area.Height / 2))))
					using (Bitmap scaled = new Bitmap(small, area.Size))
					{
						g.DrawImage(scaled, area.Location);
					}
				}
			}
		}

		private void PositionButtons()
		{
			int toolbarWidth = this.toolbarPanel.Width;
			int toolbarHeight = this.toolbarPanel.Height;
			int margin = 8;
			Rectangle sb = Screen.FromControl(this).Bounds;

			int x = this.selectionRect.Right - toolbarWidth;
			if (x < sb.Left + margin) x = sb.Left + margin;
			if (x + toolbarWidth > sb.Right - margin) x = sb.Right - toolbarWidth - margin;

			int y = this.selectionRect.Bottom + margin;
			if (y + toolbarHeight > sb.Bottom - margin)
			{
				y = this.selectionRect.Top - toolbarHeight - margin;
				if (y < sb.Top + margin)
					y = this.selectionRect.Bottom - toolbarHeight - margin;
			}

			this.toolbarPanel.Location = new Point(x, y);
			this.toolbarPanel.Visible = true;
			this.toolbarPanel.BringToFront();
		}

		private void ShowTextOverlay(Point localPos)
		{
			this.activeTextBox = new TextBox
			{
				Font = new Font("Segoe UI", 12f),
				ForeColor = this.penColor,
				BackColor = Color.FromArgb(40, 40, 44),
				BorderStyle = BorderStyle.FixedSingle,
				Location = new Point(this.selectionRect.X + localPos.X, this.selectionRect.Y + localPos.Y),
				Width = 200,
				Visible = true
			};
			this.activeTextLocalPos = localPos;
			this.activeTextBox.KeyDown += (s, ev) =>
			{
				if (ev.KeyCode == Keys.Enter)
				{
					ev.Handled = true;
					ev.SuppressKeyPress = true;
					this.CommitTextOverlay();
				}
			};
			base.Controls.Add(this.activeTextBox);
			this.activeTextBox.BringToFront();
			this.activeTextBox.Focus();
		}

		private void CommitTextOverlay()
		{
			if (this.activeTextBox == null) return;
			string txt = this.activeTextBox.Text;
			Point localPos = this.activeTextLocalPos;
			base.Controls.Remove(this.activeTextBox);
			this.activeTextBox.Dispose();
			this.activeTextBox = null;

			if (!string.IsNullOrEmpty(txt) && this.drawingBitmap != null)
			{
				this.SaveUndoState();
				using (Graphics g = Graphics.FromImage(this.drawingBitmap))
				{
					g.SmoothingMode = SmoothingMode.AntiAlias;
					g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
					using (Font f = new Font("Segoe UI", 12f))
					using (SolidBrush b = new SolidBrush(this.penColor))
					{
						g.DrawString(txt, f, b, localPos);
					}
				}
				base.Invalidate();
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if (this.darkenedScreenshot != null)
			{
				e.Graphics.DrawImage(this.darkenedScreenshot, 0, 0);
				if (this.selectionRect.Width > 0 && this.selectionRect.Height > 0)
				{
					e.Graphics.DrawImage(this.originalScreenshot, this.selectionRect, this.selectionRect, GraphicsUnit.Pixel);
					if (this.tempBitmap != null)
						e.Graphics.DrawImage(this.tempBitmap, this.selectionRect.Location);
					else if (this.drawingBitmap != null)
						e.Graphics.DrawImage(this.drawingBitmap, this.selectionRect.Location);

					using (Pen pen = new Pen(this.borderColor, 2f))
					{
						e.Graphics.DrawRectangle(pen, this.selectionRect);
					}
				}
			}
		}

		private void SaveButton_Click(object sender, EventArgs e)
		{
			this.CommitTextOverlay();
			using (Bitmap result = new Bitmap(this.selectionRect.Width, this.selectionRect.Height))
			{
				using (Graphics g = Graphics.FromImage(result))
				{
					g.DrawImage(this.originalScreenshot, 0, 0, this.selectionRect, GraphicsUnit.Pixel);
					if (this.drawingBitmap != null)
						g.DrawImage(this.drawingBitmap, 0, 0);
				}
				SaveFileDialog dlg = new SaveFileDialog
				{
					Filter = "PNG|*.png",
					FileName = this.GetNextFileName(),
					InitialDirectory = this.lastSavePath
				};
				if (dlg.ShowDialog() == DialogResult.OK)
				{
					result.Save(dlg.FileName, ImageFormat.Png);
					if (this.saveToClipboard)
						Clipboard.SetImage(result);
					this.lastSavePath = Path.GetDirectoryName(dlg.FileName);
					this.SaveLastSavePath();
					base.Close();
				}
			}
		}

		private void DrawButton_Click(object sender, EventArgs e)
		{
			this.CommitTextOverlay();
			this.currentTool = (this.currentTool == Tool.Pencil) ? Tool.None : Tool.Pencil;
			this.UpdateCursor();
		}

		private void MosaicButton_Click(object sender, EventArgs e)
		{
			this.CommitTextOverlay();
			this.currentTool = (this.currentTool == Tool.Mosaic) ? Tool.None : Tool.Mosaic;
			this.UpdateCursor();
		}

		private void ArrowButton_Click(object sender, EventArgs e)
		{
			this.CommitTextOverlay();
			this.currentTool = (this.currentTool == Tool.Arrow) ? Tool.None : Tool.Arrow;
			this.UpdateCursor();
		}

		private void TextButton_Click(object sender, EventArgs e)
		{
			this.currentTool = (this.currentTool == Tool.Text) ? Tool.None : Tool.Text;
			this.UpdateCursor();
		}

		private void ColorButton_Click(object sender, EventArgs e)
		{
			using (ColorDialog dlg = new ColorDialog())
			{
				dlg.Color = this.penColor;
				if (dlg.ShowDialog() == DialogResult.OK)
					this.penColor = dlg.Color;
			}
		}

		private void CloseButton_Click(object sender, EventArgs e)
		{
			this.CommitTextOverlay();
			base.Close();
		}

		private void MagnifierButton_Click(object sender, EventArgs e)
		{
			this.CommitTextOverlay();
			this.currentTool = (this.currentTool == Tool.Magnifier) ? Tool.None : Tool.Magnifier;
			this.UpdateCursor();
		}

		private async void OcrButton_Click(object sender, EventArgs e)
		{
			if (this.drawingBitmap == null) return;

			this.ocrButton.Enabled = false;
			try
			{
				using (var selectedBitmap = new Bitmap(this.selectionRect.Width, this.selectionRect.Height))
				{
					using (var g = Graphics.FromImage(selectedBitmap))
					{
						g.DrawImage(this.originalScreenshot, 0, 0, this.selectionRect, GraphicsUnit.Pixel);
					}

					string text = await OcrHelper.RecognizeTextAsync(selectedBitmap);

					if (string.IsNullOrWhiteSpace(text))
					{
						this.ShowOcrNotification("\u0422\u0435\u043a\u0441\u0442 \u043d\u0435 \u0440\u0430\u0441\u043f\u043e\u0437\u043d\u0430\u043d");
						return;
					}

					if (this.saveToClipboard)
					{
						var data = new DataObject();
						data.SetText(text);
						using (var imgCopy = new Bitmap(selectedBitmap))
						{
							data.SetImage(imgCopy);
							Clipboard.SetDataObject(data, true);
						}
						this.ShowOcrNotification("\u0422\u0435\u043a\u0441\u0442 \u0438 \u0441\u043d\u0438\u043c\u043e\u043a \u0441\u043a\u043e\u043f\u0438\u0440\u043e\u0432\u0430\u043d\u044b");
					}
					else
					{
						Clipboard.SetText(text);
						this.ShowOcrNotification("\u0422\u0435\u043a\u0441\u0442 \u0441\u043a\u043e\u043f\u0438\u0440\u043e\u0432\u0430\u043d \u0432 \u0431\u0443\u0444\u0435\u0440");
					}
				}
			}
			catch (Exception ex)
			{
				this.ShowOcrNotification("\u041e\u0448\u0438\u0431\u043a\u0430 OCR: " + ex.Message);
			}
			finally
			{
				this.ocrButton.Enabled = true;
			}
		}

		private void ShowOcrNotification(string message)
		{
			var label = new Label
			{
				Text = "  " + message + "  ",
				BackColor = Color.FromArgb(45, 45, 48),
				ForeColor = Color.FromArgb(220, 220, 220),
				Font = new Font("Segoe UI", 10f),
				AutoSize = true,
				Padding = new Padding(10, 6, 10, 6)
			};
			label.Location = new Point(
				this.toolbarPanel.Left,
				this.toolbarPanel.Top - label.PreferredHeight - 6);
			this.Controls.Add(label);
			label.BringToFront();

			var timer = new Timer { Interval = 2500 };
			timer.Tick += delegate
			{
				timer.Stop();
				this.Controls.Remove(label);
				label.Dispose();
				timer.Dispose();
			};
			timer.Start();
		}

		private void SaveUndoState()
		{
			if (this.drawingBitmap == null) return;
			this.undoStack.Add(new Bitmap(this.drawingBitmap));
			if (this.undoStack.Count > 30)
			{
				this.undoStack[0].Dispose();
				this.undoStack.RemoveAt(0);
			}
			foreach (var bmp in this.redoStack) bmp.Dispose();
			this.redoStack.Clear();
		}

		private void Undo()
		{
			if (this.undoStack.Count == 0 || this.drawingBitmap == null) return;
			this.redoStack.Add(new Bitmap(this.drawingBitmap));
			var prev = this.undoStack[this.undoStack.Count - 1];
			this.undoStack.RemoveAt(this.undoStack.Count - 1);
			this.drawingBitmap.Dispose();
			this.drawingBitmap = prev;
			base.Invalidate();
		}

		private void Redo()
		{
			if (this.redoStack.Count == 0 || this.drawingBitmap == null) return;
			this.undoStack.Add(new Bitmap(this.drawingBitmap));
			var next = this.redoStack[this.redoStack.Count - 1];
			this.redoStack.RemoveAt(this.redoStack.Count - 1);
			this.drawingBitmap.Dispose();
			this.drawingBitmap = next;
			base.Invalidate();
		}

		private void ClearUndoRedoStacks()
		{
			foreach (var bmp in this.undoStack) bmp.Dispose();
			this.undoStack.Clear();
			foreach (var bmp in this.redoStack) bmp.Dispose();
			this.redoStack.Clear();
		}

		private void DrawMagnifier(Graphics g, Bitmap source, Point center, int radius)
		{
			if (radius < 15) radius = 15;
			int srcHalf = radius / 2;
			Rectangle srcRect = new Rectangle(center.X - srcHalf, center.Y - srcHalf, srcHalf * 2, srcHalf * 2);
			Rectangle bounds = new Rectangle(0, 0, source.Width, source.Height);
			srcRect = Rectangle.Intersect(srcRect, bounds);
			if (srcRect.Width <= 0 || srcRect.Height <= 0) return;
			GraphicsState state = g.Save();
			using (GraphicsPath clipPath = new GraphicsPath())
			{
				clipPath.AddEllipse(center.X - radius, center.Y - radius, radius * 2, radius * 2);
				g.SetClip(clipPath, CombineMode.Intersect);
				Rectangle destRect = new Rectangle(center.X - radius, center.Y - radius, radius * 2, radius * 2);
				g.InterpolationMode = InterpolationMode.HighQualityBicubic;
				g.DrawImage(source, destRect, srcRect, GraphicsUnit.Pixel);
			}
			g.Restore(state);
			g.SmoothingMode = SmoothingMode.AntiAlias;
			using (Pen borderPen = new Pen(this.penColor, 3f))
			{
				g.DrawEllipse(borderPen, center.X - radius, center.Y - radius, radius * 2, radius * 2);
			}
			using (Pen whitePen = new Pen(Color.White, 1.5f))
			{
				g.DrawEllipse(whitePen, center.X - radius + 2, center.Y - radius + 2, (radius - 2) * 2, (radius - 2) * 2);
			}
		}

		private string GetNextFileName()
		{
			string dir = this.lastSavePath;
			string baseName = "\u0421\u043d\u0438\u043c\u043e\u043a";
			string path = Path.Combine(dir, baseName + ".png");
			int i = 1;
			while (File.Exists(path))
			{
				path = Path.Combine(dir, string.Format("{0} ({1}).png", baseName, i));
				i++;
			}
			return Path.GetFileName(path);
		}

		private class DarkShapesMenuRenderer : ToolStripProfessionalRenderer
		{
			public DarkShapesMenuRenderer() : base(new DarkShapesMenuColors()) { }
			protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
			{
				e.TextColor = Color.FromArgb(220, 220, 220);
				base.OnRenderItemText(e);
			}
		}

		private class DarkShapesMenuColors : ProfessionalColorTable
		{
			public override Color MenuItemSelected { get { return Color.FromArgb(62, 62, 66); } }
			public override Color MenuItemBorder { get { return Color.FromArgb(62, 62, 66); } }
			public override Color MenuBorder { get { return Color.FromArgb(55, 55, 60); } }
			public override Color ToolStripDropDownBackground { get { return Color.FromArgb(45, 45, 48); } }
			public override Color ImageMarginGradientBegin { get { return Color.FromArgb(45, 45, 48); } }
			public override Color ImageMarginGradientMiddle { get { return Color.FromArgb(45, 45, 48); } }
			public override Color ImageMarginGradientEnd { get { return Color.FromArgb(45, 45, 48); } }
		}

		private Rectangle selectionRect;
		private Point startPoint;
		private bool isSelecting;
		private Bitmap originalScreenshot;
		private Bitmap darkenedScreenshot;
		private Bitmap drawingBitmap;
		private Bitmap tempBitmap;
		private string lastSavePath;
		private Point lastPoint;
		private Color penColor = Color.Red;
		private Color borderColor;
		private bool saveToClipboard;
		private Tool currentTool = Tool.None;
		private Point[] quadPoints = new Point[4];
		private int quadPointCount;
		private Button saveButton;
		private Button drawButton;
		private Button arrowButton;
		private Button textButton;
		private Button colorButton;
		private Button mosaicButton;
		private Button shapesButton;
		private ContextMenuStrip shapesMenu;
		private Button magnifierButton;
		private Button ocrButton;
		private Button closeButton;
		private Panel toolbarPanel;
		private ToolTip toolTip;
		private List<Bitmap> undoStack = new List<Bitmap>();
		private List<Bitmap> redoStack = new List<Bitmap>();
		private TextBox activeTextBox;
		private Point activeTextLocalPos;

		private enum Tool
		{
			None,
			Pencil,
			Mosaic,
			Quad,
			Circle,
			Underline,
			Arrow,
			Text,
			Magnifier
		}
	}
}
