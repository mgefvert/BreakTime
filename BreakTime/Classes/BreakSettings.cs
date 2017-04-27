using System;

namespace BreakTime.Classes
{
    public class BreakSettings
    {
        // Configuration
        public TimeSpan AdditionalBreakMinutes { get; set; } = TimeSpan.FromMinutes(2);
        public bool AllowClosing { get; set; } = true;
        public bool AllowSnoozing { get; set; } = true;
        public TimeSpan MainBreakInterval { get; set; } = TimeSpan.FromMinutes(60);
        public TimeSpan MainBreakMinutes { get; set;  } = TimeSpan.FromMinutes(3);
        public TimeSpan SnoozeTime { get; set; } = new TimeSpan(0, 5, 0);
        public TimeSpan TimeStart { get; set; } = new TimeSpan(8, 0, 0);
        public TimeSpan TimeStop { get; set; } = new TimeSpan(18, 0, 0);
        public bool UseAdditionalBreak { get; set; } = false;
        public bool UseHours { get; set; } = true;

        // State
        public DateTime LastBreakTime { get; set; } = DateTime.Now;
        public bool AdditionalBreakDone { get; set; }

        public int AdditionalBreakMinutesValue
        {
            get => (int)AdditionalBreakMinutes.TotalMinutes;
            set => AdditionalBreakMinutes = TimeSpan.FromMinutes(value);
        }

        public int MainBreakIntervalValue
        {
            get => (int)MainBreakInterval.TotalMinutes;
            set => MainBreakInterval = TimeSpan.FromMinutes(value);
        }

        public int MainBreakMinutesValue
        {
            get => (int)MainBreakMinutes.TotalMinutes;
            set => MainBreakMinutes = TimeSpan.FromMinutes(value);
        }

        public int TimeStartValue
        {
            get => (int)TimeStart.TotalHours;
            set => TimeStart = TimeSpan.FromHours(value);
        }

        public int TimeStopValue
        {
            get => (int)TimeStop.TotalHours;
            set => TimeStop = TimeSpan.FromHours(value);
        }

        public BreakSettings Clone()
        {
            return (BreakSettings) MemberwiseClone();
        }
    }
}
