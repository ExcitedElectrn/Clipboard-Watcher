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
        private readonly PopupStyleSettings style;

        public ToastForm(string message, PopupStyleSettings style = null)
        {
            this.message = message;
            this.style = style ?? PopupStyleSettings.Load();
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            TopMost = true;
            DoubleBuffered = true;
            Width = 228;
            Height = 56;
            Padding = new Padding(0);
            BackColor = Color.Black;
            Opacity = 0.88;

            timer = new Timer { Interval = 2200 };
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                Close();
            };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Rectangle screen = Screen.PrimaryScreen.WorkingArea;
            Location = new Point((screen.Width - Width) / 2, screen.Height - Height - 40);

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

            Rectangle rect = new Rectangle(1, 1, Width - 2, Height - 2);

            using (GraphicsPath path = RoundedRect(rect, style.CornerRadius))
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
            using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center })
            using (SolidBrush textBrush = new SolidBrush(Color.FromArgb(245, 255, 255, 255)))
            using (Font textFont = new Font("Segoe UI Semibold", style.FontSize, FontStyle.Regular))
            {
                g.DrawString(message, textFont, textBrush, textRect, sf);
            }
        }

        private void DrawLiquidGlassBackground(Graphics g, Rectangle rect, GraphicsPath path)
        {
            Color brightColor = Mix(style.PopupColor, Color.White, 0.35f, 120);
            Color deepColor = Mix(style.PopupColor, Color.FromArgb(120, 180, 255), 0.2f, 92);

            using (LinearGradientBrush glassBrush = new LinearGradientBrush(rect, brightColor, deepColor, LinearGradientMode.ForwardDiagonal))
            {
                g.FillPath(glassBrush, path);
            }

            Rectangle innerRect = new Rectangle(rect.X + 2, rect.Y + 2, rect.Width - 4, rect.Height - 4);
            using (GraphicsPath innerPath = RoundedRect(innerRect, Math.Max(8, style.CornerRadius - 2)))
            using (LinearGradientBrush innerBrush = new LinearGradientBrush(
                innerRect,
                Color.FromArgb(62, 255, 255, 255),
                Color.FromArgb(24, 255, 255, 255),
                LinearGradientMode.Vertical))
            {
                g.FillPath(innerBrush, innerPath);
            }
        }

        private void DrawGlassHighlight(Graphics g, Rectangle rect, GraphicsPath path)
        {
            Rectangle topGlowRect = new Rectangle(rect.X + 6, rect.Y + 4, rect.Width - 12, rect.Height / 2);
            using (GraphicsPath glowPath = RoundedRect(topGlowRect, Math.Max(8, style.CornerRadius - 6)))
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
                Mix(style.PopupColor, Color.White, 0.5f, 58),
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
            Region?.Dispose();
            using (GraphicsPath rounded = RoundedRect(new Rectangle(0, 0, Width, Height), style.CornerRadius + 2))
            {
                Region = new Region(rounded);
            }
        }

        private static Color Mix(Color first, Color second, float ratio, int alpha)
        {
            ratio = Math.Max(0f, Math.Min(1f, ratio));
            int r = (int)(first.R * (1f - ratio) + second.R * ratio);
            int g = (int)(first.G * (1f - ratio) + second.G * ratio);
            int b = (int)(first.B * (1f - ratio) + second.B * ratio);
            return Color.FromArgb(alpha, r, g, b);
        }

        private GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int safeRadius = Math.Max(2, Math.Min(radius, Math.Min(bounds.Width, bounds.Height) / 2));
            int d = safeRadius * 2;
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
