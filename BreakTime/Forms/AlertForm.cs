using System;
using System.Drawing;
using System.Windows.Forms;
using DotNetCommons.WinForms;

// ReSharper disable LocalizableElement

namespace BreakTime.Forms
{
    public partial class AlertForm : Form
    {
        private readonly Screen _screen;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WinApi.WM_NCHITTEST)
            {
                m.Result = (IntPtr)WinApi.HITTEST.HTTRANSPARENT;
                return;
            }

            base.WndProc(ref m);
        }

        public void UpdateState(TimeSpan? left)
        {
            if (left == null || left.Value.TotalMinutes < 0 || left.Value.TotalMinutes >= 3)
            {
                Visible = false;
                return;
            }

            label1.Text = "Break in " + left.Value.ToString("mm':'ss");
            Visible = true;

            if (left.Value.TotalSeconds < 20)
            {
                Opacity = (int) left.Value.TotalMilliseconds % 1000 <= 500 ? 1 : 0.1;
                BackColor = Color.DarkRed;
            }
            else
            {
                Opacity = 0.5;
                BackColor = Color.DimGray;
            }

            ResizeWindow();
        }

        public AlertForm(Screen screen)
        {
            InitializeComponent();
            _screen = screen;
            ResizeWindow();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var result = base.CreateParams;
                result.ExStyle |= WinApi.WS_EX_TRANSPARENT;
                return result;
            }
        }

        private void ResizeWindow()
        {
            var b = _screen.Bounds;
            SetBounds(b.Right - Width - 34, b.Bottom - Height - 70, 0, 0, BoundsSpecified.Location);
        }

        private void ExtraForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
