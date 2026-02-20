using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ToastMessageWhenCopied
{
    public partial class Form1 : Form
    {
        private readonly NotifyIcon trayIcon;
        private readonly ContextMenuStrip trayMenu;
        private PopupStyleSettings popupStyle;

        public Form1()
        {
            InitializeComponent();
            popupStyle = PopupStyleSettings.Load();

            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("Customize...", null, OnCustomizeClicked);
            trayMenu.Items.Add(new ToolStripSeparator());
            trayMenu.Items.Add("Exit", null, OnExitClicked);

            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream("ToastMessageWhenCopied.copied_icon_outlined.ico"))
            {
                trayIcon = new NotifyIcon
                {
                    Icon = new Icon(stream),
                    Visible = true,
                    Text = "Clipboard Watcher",
                    ContextMenuStrip = trayMenu
                };
            }

            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;

            NativeMethods.AddClipboardFormatListener(Handle);
        }

        private void OnCustomizeClicked(object sender, EventArgs e)
        {
            using (CustomizeForm customizeForm = new CustomizeForm(popupStyle))
            {
                if (customizeForm.ShowDialog() == DialogResult.OK)
                {
                    popupStyle = customizeForm.SelectedStyle;
                    popupStyle.Save();
                    new ToastForm("Style saved", popupStyle).Show();
                }
            }
        }

        private void OnExitClicked(object sender, EventArgs e)
        {
            trayIcon.Visible = false;
            Application.Exit();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == NativeMethods.WM_CLIPBOARDUPDATE)
            {
                if (Clipboard.ContainsText() || Clipboard.ContainsImage() || Clipboard.ContainsFileDropList())
                {
                    new ToastForm("Copied!", popupStyle).Show();
                }
            }

            base.WndProc(ref m);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            NativeMethods.RemoveClipboardFormatListener(Handle);
            trayIcon.Visible = false;
            base.OnFormClosing(e);
        }

        internal static class NativeMethods
        {
            public const int WM_CLIPBOARDUPDATE = 0x031D;
            [DllImport("user32.dll")] public static extern bool AddClipboardFormatListener(IntPtr hwnd);
            [DllImport("user32.dll")] public static extern bool RemoveClipboardFormatListener(IntPtr hwnd);
        }
    }
}
