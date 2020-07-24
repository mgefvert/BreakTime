using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BreakTime.Classes;

namespace BreakTime.Forms
{
    public partial class DebugForm : Form
    {
        private readonly BreakController _controller;

        public DebugForm(BreakController controller)
        {
            InitializeComponent();
            _controller = controller;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!Visible)
            {
                label1.Text = null;
                return;
            }

            var minutes = (DateTime.Now - _controller.LastBreak).TotalMinutes;

            var sb = new StringBuilder();
            sb.AppendLine($"Movement counter: {_controller.Movement.Counter}");
            sb.AppendLine($"Movement/minute: {(int)(_controller.Movement.Counter/minutes)}");
            sb.AppendLine($"Minutes from last break: {minutes:F2}");

            sb.AppendLine();
            sb.AppendLine("Movement per 5 minutes:");
            foreach (var item in _controller.Movement.History.OrderBy(x => x.Key))
                sb.AppendLine($"{item.Key}: {item.Value}");

            sb.AppendLine();
            sb.AppendLine("Meetings today:");
            foreach (var meeting in _controller.Meetings)
                sb.AppendLine($"{meeting.Start:HH-mm} - {meeting.End:HH-mm} {meeting.Name}");

            label1.Text = sb.ToString();
        }
    }
}
