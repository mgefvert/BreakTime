using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;

namespace BreakTime
{
    public partial class MainForm : Form
    {
        private readonly BreakController _breakController;
        private readonly List<ExtraForm> _extraForms = new List<ExtraForm>();

        public MainForm()
        {
            InitializeComponent();

            _breakController = new BreakController
            {
                Notifier = notifyIcon1,
                BreakForm = this
            };
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            _breakController.Tick();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            BackgroundImage = GenerateBackgroundImage();
        }

        public static Bitmap GenerateBackgroundImage()
        {
            var result = new Bitmap(256, 256, PixelFormat.Format32bppArgb);

            using (var g = Graphics.FromImage(result))
            using (var pen1 = new Pen(Color.FromArgb(64, 64, 64)))
            using (var pen2 = new Pen(Color.FromArgb(60, 60, 60)))
            {
                for (var y = 0; y < result.Height; y++)
                {
                    var x1 = -y % 128;
                    var c = Math.Abs(y / 128) % 2;

                    while (x1 - 128 < result.Width)
                    {
                        g.DrawLine(c == 0 ? pen1 : pen2, x1 - 128, y, x1, y);

                        x1 += 128;
                        c = 1 - c;
                    }
                }
            }

            return result;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            label1.Left = (ClientSize.Width - label1.Width) / 2;
            label1.Top = (ClientSize.Height - label1.Height) / 2;

            button1.Left = (ClientSize.Width - button1.Width) / 2;
            button1.Top = label1.Top + label1.Height + 20;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            var left = (_breakController.EndOfBreak - DateTime.Now).ToString(@"m':'ss");
            label1.Text = left;
        }

        private void Form1_VisibleChanged(object sender, EventArgs e)
        {
            ClearExtraForms();

            timer2_Tick(this, EventArgs.Empty);
            timer2.Enabled = Visible;

            if (Visible)
            {
                button1.Visible = _breakController.SnoozeAllowed && _breakController.Settings.AllowSnoozing;

                foreach(var screen in Screen.AllScreens)
                    if (!screen.Primary)
                    {
                        var form = new ExtraForm();
                        form.SetBounds(screen.Bounds.X, screen.Bounds.Y, screen.Bounds.Width, screen.Bounds.Height, BoundsSpecified.All);
                        form.Show();

                        _extraForms.Add(form);
                    }
            }
        }

        private void ClearExtraForms()
        {
            foreach(var form in _extraForms)
                form.Dispose();

            _extraForms.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _breakController.Snooze();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            closeToolStripMenuItem.Enabled = _breakController.Settings.AllowClosing;
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var forms = Application.OpenForms.Cast<Form>().ToList();
            foreach(var form in forms)
                form.Dispose();

            Application.Exit();
        }
    }
}
