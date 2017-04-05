using System;

namespace BreakTime
{
    public class BreakSettings
    {
        public TimeSpan AdditionalBreakMinutes { get; set; } = TimeSpan.FromMinutes(2);
        public bool AllowClosing { get; set; } = true;
        public bool AllowSnoozing { get; set; } = true;
        public TimeSpan MainBreakInterval { get; set; } = TimeSpan.FromMinutes(60);
        public TimeSpan MainBreakMinutes { get; set;  } = TimeSpan.FromMinutes(3);
        public TimeSpan SnoozeTime { get; set; } = new TimeSpan(0, 3, 0);
        public TimeSpan TimeStart { get; set; } = new TimeSpan(8, 0, 0);
        public TimeSpan TimeStop { get; set; } = new TimeSpan(18, 0, 0);
        public bool UseAdditionalBreak { get; set; } = true;
        public bool UseHours { get; set; } = true;
    }
}
