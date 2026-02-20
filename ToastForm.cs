using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ToastMessageWhenCopied
{
    public partial class ToastForm : Form
    {
        private readonly Timer timer;
        private readonly string message;

        public ToastForm(string message)
        {
            this.message = message;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.DoubleBuffered = true;
            this.Width = 228;
            this.Height = 56;
            this.Padding = new Padding(0);
            this.BackColor = Color.Black;
            this.Opacity = 0.88;

            timer = new Timer { Interval = 2200 };
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                this.Close();
            };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Rectangle screen = Screen.PrimaryScreen.WorkingArea;
            this.Location = new Point(
                (screen.Width - this.Width) / 2,
                screen.Height - this.Height - 40);

            UpdateRoundedRegion();
            timer.Start();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            UpdateRoundedRegion();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            Rectangle rect = new Rectangle(1, 1, this.Width - 2, this.Height - 2);

            using (GraphicsPath path = RoundedRect(rect, 18))
            {
                DrawLiquidGlassBackground(g, rect, path);
                DrawGlassHighlight(g, rect, path);
                using (Pen borderPen = new Pen(Color.FromArgb(145, 255, 255, 255), 1.1f))
                {
                    g.DrawPath(borderPen, path);
                }
            }

            DrawCheckIcon(g, rect);

            Rectangle textRect = new Rectangle(54, 0, rect.Width - 58, rect.Height);
            using (StringFormat sf = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center
            })
            using (SolidBrush textBrush = new SolidBrush(Color.FromArgb(245, 255, 255, 255)))
            using (Font textFont = new Font("Segoe UI Semibold", 10.2f, FontStyle.Regular))
            {
                g.DrawString(message, textFont, textBrush, textRect, sf);
            }
        }

        private void DrawLiquidGlassBackground(Graphics g, Rectangle rect, GraphicsPath path)
        {
            using (LinearGradientBrush glassBrush = new LinearGradientBrush(
                rect,
                Color.FromArgb(120, 180, 214, 255),
                Color.FromArgb(95, 197, 178, 255),
                LinearGradientMode.ForwardDiagonal))
            {
                g.FillPath(glassBrush, path);
            }

            Rectangle innerRect = new Rectangle(rect.X + 2, rect.Y + 2, rect.Width - 4, rect.Height - 4);
            using (GraphicsPath innerPath = RoundedRect(innerRect, 16))
            using (LinearGradientBrush innerBrush = new LinearGradientBrush(
                innerRect,
                Color.FromArgb(60, 255, 255, 255),
                Color.FromArgb(30, 255, 255, 255),
                LinearGradientMode.Vertical))
            {
                g.FillPath(innerBrush, innerPath);
            }
        }

        private void DrawGlassHighlight(Graphics g, Rectangle rect, GraphicsPath path)
        {
            Rectangle topGlowRect = new Rectangle(rect.X + 6, rect.Y + 4, rect.Width - 12, rect.Height / 2);
            using (GraphicsPath glowPath = RoundedRect(topGlowRect, 12))
            using (LinearGradientBrush glowBrush = new LinearGradientBrush(
                topGlowRect,
                Color.FromArgb(120, 255, 255, 255),
                Color.FromArgb(8, 255, 255, 255),
                LinearGradientMode.Vertical))
            {
                Region previousClip = g.Clip;
                g.SetClip(path);
                g.FillPath(glowBrush, glowPath);
                g.Clip = previousClip;
            }
        }

        private void DrawCheckIcon(Graphics g, Rectangle rect)
        {
            Rectangle iconBounds = new Rectangle(16, (rect.Height - 24) / 2, 24, 24);

            using (LinearGradientBrush iconBrush = new LinearGradientBrush(
                iconBounds,
                Color.FromArgb(120, 255, 255, 255),
                Color.FromArgb(55, 220, 240, 255),
                LinearGradientMode.Vertical))
            using (Pen iconBorder = new Pen(Color.FromArgb(120, 255, 255, 255), 1f))
            using (Pen checkPen = new Pen(Color.White, 2.6f))
            {
                g.FillEllipse(iconBrush, iconBounds);
                g.DrawEllipse(iconBorder, iconBounds);

                checkPen.StartCap = LineCap.Round;
                checkPen.EndCap = LineCap.Round;

                Point p1 = new Point(iconBounds.X + 6, iconBounds.Y + 13);
                Point p2 = new Point(iconBounds.X + 10, iconBounds.Y + 17);
                Point p3 = new Point(iconBounds.X + 18, iconBounds.Y + 8);
                g.DrawLines(checkPen, new[] { p1, p2, p3 });
            }
        }

        private void UpdateRoundedRegion()
        {
            this.Region?.Dispose();
            using (GraphicsPath rounded = RoundedRect(new Rectangle(0, 0, Width, Height), 20))
            {
                this.Region = new Region(rounded);
            }
        }

        private GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int d = radius * 2;
            GraphicsPath path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
