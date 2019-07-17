using System;
using System.Drawing;
using System.Windows.Forms;

namespace BreakTime.Forms
{
    public partial class ExtraForm : Form
    {
        private readonly Screen _screen;

        public ExtraForm(Screen screen)
        {
            InitializeComponent();
            _screen = screen;
            ResizeWindow();
        }

        private void ResizeWindow()
        {
            var b = _screen.Bounds;
            SetBounds(b.X, b.Y, b.Width, b.Height, BoundsSpecified.All);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            BackgroundImage = MainForm.GenerateBackgroundImage(Color.FromArgb(64, 64, 64), Color.FromArgb(60, 60, 60));
        }

        private void ExtraForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
