using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
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
            _fortuneLabel.Text = "";  // Blank out fortune cookie placeholder

            _hotkeys = new Hotkeys(Handle);
            _hotkeys.Add(WinApi.MOD_CONTROL | WinApi.MOD_WIN, (uint) Keys.F12, () => _breakController.BreakNow(BreakType.Main));

            _breakController = new BreakController
            {
                Notifier = notifyIcon1,
                BreakForm = this,
                AlertForm = new AlertForm(Screen.PrimaryScreen)
            };
            _breakController.LoadSettings();

            ResizeWindows();
        }

        protected override void WndProc(ref Message msg)
        {
            switch (msg.Msg)
            {
                case (int)WinApi.WM.HOTKEY:
                    _hotkeys.Process(ref msg);
                    return;

                case (int)WinApi.WM.POWERBROADCAST:
                    var reason = (int)msg.WParam;
                    if (reason == 0x18 || reason == 7) // Resumed from sleep
                        _breakController.Reset();
                    return;
            }

            if (msg.Msg == (int)WinApi.WM.HOTKEY)
                _hotkeys.Process(ref msg);
            else
                base.WndProc(ref msg);
        }

        private void ClearExtraForms()
        {
            foreach(var form in _extraForms)
                form.Dispose();

            _extraForms.Clear();
        }

        private void CloseApplication()
        {
            _breakController.SaveSettings();

            var forms = Application.OpenForms.Cast<Form>().ToList();
            foreach (var form in forms)
                form.Dispose();

            Application.Exit();
        }

        public static Bitmap GenerateBackgroundImage(Color color1, Color color2)
        {
            var result = new Bitmap(256, 256, PixelFormat.Format32bppArgb);

            using (var g = Graphics.FromImage(result))
            using (var pen1 = new Pen(color1))
            using (var pen2 = new Pen(color2))
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

        private void ResizeWindows()
        {
            var b = Screen.PrimaryScreen.Bounds;
            SetBounds(b.X, b.Y, b.Width, b.Height, BoundsSpecified.All);

            _timeLabel.Left = (ClientSize.Width - _timeLabel.Width) / 2;
            _timeLabel.Top = (ClientSize.Height - _timeLabel.Height) / 2;

            _snoozeButton.Left = (ClientSize.Width - _snoozeButton.Width) / 2;
            _snoozeButton.Top = _timeLabel.Top + _timeLabel.Height + 20;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _breakController.SaveSettings();
            e.Cancel = true;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            BackgroundImage = GenerateBackgroundImage(Color.FromArgb(64, 64, 64), Color.FromArgb(60, 60, 60));
            _breakController.LoadSettings();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            ResizeWindows();
        }

        private void MainForm_VisibleChanged(object sender, EventArgs e)
        {
            ClearExtraForms();

            displayTimer_Tick(this, EventArgs.Empty);
            displayTimer.Enabled = Visible;

            if (Visible)
            {
                ResizeWindows();

                var fortune = _fortunes.Random();
                _fortuneLabel.Text = fortune != null ? "“" + fortune + "”" : null;
                _snoozeButton.Visible = _breakController.SnoozeAllowed && _breakController.Settings.AllowSnoozing;

                foreach (var screen in Screen.AllScreens)
                    if (!screen.Primary)
                    {
                        var form = new ExtraForm(screen);
                        form.Show();

                        _extraForms.Add(form);
                    }
            }
            else
                _fortuneLabel.Text = "";

            ThreadPool.QueueUserWorkItem(state => { _breakController.SaveSettings(); });
        }

        private void breakTimer_Tick(object sender, EventArgs e)
        {
            var left = _breakController.Tick();

            notifyIcon1.Text = "BreakTime" +
                               (left != null ? $" - break in {(int)left.Value.TotalMinutes} minutes" : "");
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseApplication();
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            closeToolStripMenuItem.Enabled = _breakController.Settings.AllowClosing;
        }

        private void displayTimer_Tick(object sender, EventArgs e)
        {
            var left = (_breakController.EndOfBreak - DateTime.Now).ToString(@"m':'ss");
            _timeLabel.Text = left;
        }

        private void snoozeButton_Click(object sender, EventArgs e)
        {
            _breakController.Snooze();
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
