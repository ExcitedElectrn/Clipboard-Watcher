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
            this.Width = 220;
            this.Height = 52;
            this.Padding = new Padding(0);
            this.BackColor = Color.Black;
            this.Opacity = 0.96;

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
            using (LinearGradientBrush backgroundBrush = new LinearGradientBrush(
                rect,
                Color.FromArgb(242, 79, 70, 229),
                Color.FromArgb(242, 45, 176, 248),
                LinearGradientMode.Horizontal))
            using (Pen borderPen = new Pen(Color.FromArgb(165, 255, 255, 255), 1.2f))
            {
                g.FillPath(backgroundBrush, path);
                g.DrawPath(borderPen, path);
            }

            DrawCheckIcon(g, rect);

            Rectangle textRect = new Rectangle(52, 0, rect.Width - 56, rect.Height);
            using (StringFormat sf = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center
            })
            using (SolidBrush textBrush = new SolidBrush(Color.White))
            using (Font textFont = new Font("Segoe UI Semibold", 10.2f, FontStyle.Regular))
            {
                g.DrawString(message, textFont, textBrush, textRect, sf);
            }
        }

        private void DrawCheckIcon(Graphics g, Rectangle rect)
        {
            Rectangle iconBounds = new Rectangle(14, (rect.Height - 24) / 2, 24, 24);

            using (SolidBrush iconBrush = new SolidBrush(Color.FromArgb(78, 255, 255, 255)))
            using (Pen checkPen = new Pen(Color.White, 2.6f))
            {
                g.FillEllipse(iconBrush, iconBounds);
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
