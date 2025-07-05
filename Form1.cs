using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Toolkit.Uwp.Notifications;

namespace ToastMessageWhenCopied
{
    public partial class Form1 : Form
    {
        //public Form1()
        //{
        //    InitializeComponent();
        //}

        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;

        public Form1()
        {
            InitializeComponent();

            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("Exit", null, OnExitClicked);

            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream("ToastMessageWhenCopied.copied_icon_outlined.ico"))
            {
                // Create tray icon
                trayIcon = new NotifyIcon
                {
                    Icon = new Icon(stream),
                    //Icon = System.Drawing.SystemIcons.Information, // Or use your own .ico file
                    Visible = true,
                    Text = "Clipboard Watcher",
                    ContextMenuStrip = trayMenu
                };
            }

            // Hide main window
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;

            // Listen to clipboard
            NativeMethods.AddClipboardFormatListener(this.Handle);
        }

        private void OnExitClicked(object sender, EventArgs e)
        {
            trayIcon.Visible = false; // Hide icon before exit
            Application.Exit();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == NativeMethods.WM_CLIPBOARDUPDATE)
            {

                if (Clipboard.ContainsText() || Clipboard.ContainsImage() || Clipboard.ContainsFileDropList())
                {
                    new ToastForm("Copied!").Show();
                }

                //if (Clipboard.ContainsText())
                //{
                //    string copiedText = "Copied!";//Clipboard.GetText();

                //    //new ToastContentBuilder()
                //    //    .AddText("Copied to Clipboard")
                //    //    .AddText(copiedText)
                //    //    .Show();

                //    new ToastForm(copiedText).Show();
                //}

                //new ToastForm(copiedText).Show();


            }
            base.WndProc(ref m);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            NativeMethods.RemoveClipboardFormatListener(this.Handle);
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
