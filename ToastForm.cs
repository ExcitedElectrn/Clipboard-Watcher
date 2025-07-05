using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ToastMessageWhenCopied
{
    public partial class ToastForm : Form
    {
        private Timer timer;
        private string message;

        public ToastForm(string message)
        {
            this.message = message;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.BackColor = Color.White; // any color — won't be shown
            this.TransparencyKey = this.BackColor;
            this.DoubleBuffered = true;
            this.Width = 100;
            this.Height = 20;

            // Auto-close timer
            timer = new Timer { Interval = 2000 };
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                this.Close();
            };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Position bottom center
            Rectangle screen = Screen.PrimaryScreen.WorkingArea;
            this.Location = new Point(
                (screen.Width - this.Width) / 2,
                screen.Height - this.Height - 40);

            timer.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int radius = 20;
            Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);
            GraphicsPath path = RoundedRect(rect, radius);

            // Semi-transparent dark background
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(180, 189, 160, 238))) // A=180, dark gray
                g.FillPath(brush, path);

            // Centered white text
            using (StringFormat sf = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            })
            using (SolidBrush textBrush = new SolidBrush(Color.Black))
            {
                g.DrawString(message, new Font("Segoe UI", 10), textBrush, rect, sf);
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
