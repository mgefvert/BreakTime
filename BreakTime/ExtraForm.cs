using System;
using System.Windows.Forms;

namespace BreakTime
{
    public partial class ExtraForm : Form
    {
        public ExtraForm()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            BackgroundImage = MainForm.GenerateBackgroundImage();
        }

        private void ExtraForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
