using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using BreakTime.Classes;
using DotNetCommons.WinForms;
// ReSharper disable LocalizableElement

namespace BreakTime.Forms
{
    public partial class MainForm : Form
    {
        private readonly BreakController _breakController;
        private readonly List<ExtraForm> _extraForms = new List<ExtraForm>();
        private readonly Hotkeys _hotkeys;
        private readonly Fortunes _fortunes;

        public MainForm()
        {
            InitializeComponent();

            _fortunes = new Fortunes(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".", "fortunes"));
            fortuneLabel.Text = "";  // Blank out fortune cookie placeholder

            _hotkeys = new Hotkeys(Handle);
            _hotkeys.Add(WinApi.MOD_CONTROL | WinApi.MOD_WIN, (uint) Keys.F12, () => _breakController.BreakNow(BreakType.Main));
            
            _breakController = new BreakController
            {
                Notifier = notifyIcon1,
                BreakForm = this
            };
            _breakController.LoadSettings();

            Form1_Resize(this, EventArgs.Empty);
        }

        protected override void WndProc(ref Message msg)
        {
            switch (msg.Msg)
            {
                case WinApi.WM_HOTKEY:
                    _hotkeys.Process(ref msg);
                    return;

                case WinApi.WM_POWERBROADCAST:
                    var reason = (int)msg.WParam;
                    if (reason == 0x18 || reason == 7) // Resumed from sleep
                        _breakController.Reset();
                    return;
            }

            if (msg.Msg == WinApi.WM_HOTKEY)
                _hotkeys.Process(ref msg);
            else
                base.WndProc(ref msg);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var left = _breakController.Tick();

            notifyIcon1.Text = "BreakTime" +
                               (left != null ? $" - break in {(int) left.Value.TotalMinutes} minutes" : "");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            BackgroundImage = GenerateBackgroundImage();
            _breakController.LoadSettings();
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
            timeLabel.Left = (ClientSize.Width - timeLabel.Width) / 2;
            timeLabel.Top = (ClientSize.Height - timeLabel.Height) / 2;

            snoozeButton.Left = (ClientSize.Width - snoozeButton.Width) / 2;
            snoozeButton.Top = timeLabel.Top + timeLabel.Height + 20;
        }

        private void Form1_VisibleChanged(object sender, EventArgs e)
        {
            Form1_Resize(this, EventArgs.Empty);
            ClearExtraForms();
            _breakController.SaveSettings();

            timer2_Tick(this, EventArgs.Empty);
            timer2.Enabled = Visible;

            if (Visible)
            {
                var fortune = _fortunes.Random();
                fortuneLabel.Text = fortune != null ? "“" + fortune + "”" : null;

                snoozeButton.Visible = _breakController.SnoozeAllowed && _breakController.Settings.AllowSnoozing;

                foreach (var screen in Screen.AllScreens)
                    if (!screen.Primary)
                    {
                        var form = new ExtraForm();
                        form.SetBounds(screen.Bounds.X, screen.Bounds.Y, screen.Bounds.Width, screen.Bounds.Height,
                            BoundsSpecified.All);
                        form.Show();

                        _extraForms.Add(form);
                    }
            }
            else
                fortuneLabel.Text = "";
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            var left = (_breakController.EndOfBreak - DateTime.Now).ToString(@"m':'ss");
            timeLabel.Text = left;
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
            _breakController.SaveSettings();
            e.Cancel = true;
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            closeToolStripMenuItem.Enabled = _breakController.Settings.AllowClosing;
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _breakController.SaveSettings();

            var forms = Application.OpenForms.Cast<Form>().ToList();
            foreach(var form in forms)
                form.Dispose();

            Application.Exit();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var settings = _breakController.Settings.Clone();

            using (var form = new SettingsForm(settings))
            {
                if (form.ShowDialog() == DialogResult.OK)
                    _breakController.Settings = settings;
            }
        }
    }
}
