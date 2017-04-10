using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BreakTime.Classes;

namespace BreakTime.Forms
{
    public partial class SettingsForm: Form
    {
        private readonly BreakSettings _settings;

        public SettingsForm(BreakSettings settings)
        {
            InitializeComponent();
            _settings = settings;

            breakSettingsBindingSource.Add(_settings);
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {

        }
    }
}
