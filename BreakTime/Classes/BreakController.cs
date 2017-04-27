using System;
using System.Linq;
using System.Windows.Forms;
using DotNetCommons;
using DotNetCommons.WinForms;
using Microsoft.Win32;
using Stateless;

namespace BreakTime.Classes
{
    public enum BreakType
    {
        None,
        Main,
        Additional
    }

    public enum BreakState
    {
        Waiting,
        Alert3Minutes,
        Alert10Seconds,
        Breaking,
        BreakingFirstTime,
        BreakingAfterSnoozing1,
        BreakingAfterSnoozing2,
        Snoozing,
        Snoozing1,
        Snoozing2
    }

    public enum BreakTrigger
    {
        Alert3Minutes,
        Alert10Seconds,
        Break,
        Snooze,
        Completed
    }

    public class BreakController
    {
        private readonly StateMachine<BreakState, BreakTrigger> _stateMachine;
        private BreakSettings _settings = new BreakSettings();

        public BreakSettings Settings
        {
            get => _settings;
            set {
                _settings = value;
                SaveSettings();
            }
        }

        public DateTime EndOfBreak { get; private set; }
        public DateTime EndOfSnooze { get; private set; }
        private BreakType _currentBreakType = BreakType.Main;

        public NotifyIcon Notifier { get; set; }
        public Form BreakForm { get; set; }

        public bool SnoozeAllowed => _stateMachine.PermittedTriggers.Any(x => x == BreakTrigger.Snooze);

        public BreakController()
        {
            _stateMachine = new StateMachine<BreakState, BreakTrigger>(BreakState.Waiting);
            _stateMachine.OnUnhandledTrigger(delegate { /* Ignore */ });

            // Wait states

            _stateMachine.Configure(BreakState.Waiting)
                .Permit(BreakTrigger.Break, BreakState.BreakingFirstTime)
                .Permit(BreakTrigger.Alert3Minutes, BreakState.Alert3Minutes)
                .Permit(BreakTrigger.Alert10Seconds, BreakState.Alert10Seconds);

            _stateMachine.Configure(BreakState.Alert3Minutes)
                .SubstateOf(BreakState.Waiting)
                .OnEntry(() => Notify("Upcoming break in 3 minutes."))
                .Ignore(BreakTrigger.Alert3Minutes);

            _stateMachine.Configure(BreakState.Alert10Seconds)
                .SubstateOf(BreakState.Waiting)
                .OnEntry(() => Notify("A break is imminent. Stop working.", 9000))
                .Ignore(BreakTrigger.Alert3Minutes)
                .Ignore(BreakTrigger.Alert10Seconds);

            // Breaking states

            _stateMachine.Configure(BreakState.Breaking)
                .OnEntry(BreakStart)
                .OnExit(BreakEnd)
                .Permit(BreakTrigger.Completed, BreakState.Waiting);

            _stateMachine.Configure(BreakState.BreakingFirstTime)
                .SubstateOf(BreakState.Breaking)
                .Permit(BreakTrigger.Snooze, BreakState.Snoozing1);

            _stateMachine.Configure(BreakState.BreakingAfterSnoozing1)
                .SubstateOf(BreakState.Breaking)
                .Permit(BreakTrigger.Snooze, BreakState.Snoozing2);

            _stateMachine.Configure(BreakState.BreakingAfterSnoozing2)
                .SubstateOf(BreakState.Breaking);

            // Snoozing state

            _stateMachine.Configure(BreakState.Snoozing)
                .OnEntry(SnoozeStart)
                .Permit(BreakTrigger.Completed, BreakState.Waiting);

            _stateMachine.Configure(BreakState.Snoozing1)
                .SubstateOf(BreakState.Snoozing)
                .Permit(BreakTrigger.Break, BreakState.BreakingAfterSnoozing1);

            _stateMachine.Configure(BreakState.Snoozing2)
                .SubstateOf(BreakState.Snoozing)
                .Permit(BreakTrigger.Break, BreakState.BreakingAfterSnoozing2);
        }

        private void BreakEnd()
        {
            BreakForm.Hide();

            if (_currentBreakType == BreakType.Main)
            {
                _settings.LastBreakTime = DateTime.Now;
                _settings.AdditionalBreakDone = false;
            }
            else if (_currentBreakType == BreakType.Additional)
                _settings.AdditionalBreakDone = true;
        }

        public void BreakNow(BreakType breakType)
        {
            _currentBreakType = breakType;
            _stateMachine.Fire(BreakTrigger.Break);
        }

        private void BreakStart()
        {
            EndOfBreak = DateTime.Now.Add(_currentBreakType == BreakType.Main ? Settings.MainBreakMinutes : Settings.AdditionalBreakMinutes);
            BreakForm.Show();
        }

        public void Reset()
        {
            Settings.LastBreakTime = DateTime.Now;
            Settings.AdditionalBreakDone = false;

            _stateMachine.Fire(BreakTrigger.Completed);
        }

        public Tuple<BreakType, DateTime> NextBreak()
        {
            var now = DateTime.Now;
            if (Settings.UseHours)
            {
                bool active;
                if (Settings.TimeStart < Settings.TimeStop)
                    active = Settings.TimeStart <= now.TimeOfDay && Settings.TimeStop > now.TimeOfDay;
                else
                    active = Settings.TimeStart <= now.TimeOfDay || Settings.TimeStop > now.TimeOfDay;

                if (!active)
                    return new Tuple<BreakType, DateTime>(BreakType.None, DateTime.MaxValue);
            }

            var main = _settings.LastBreakTime.Add(Settings.MainBreakInterval);
            var additional = Settings.UseAdditionalBreak && !_settings.AdditionalBreakDone
                ? _settings.LastBreakTime.AddSeconds(Settings.MainBreakInterval.TotalSeconds / 2)
                : DateTime.MaxValue;

            if (additional < main)
                return new Tuple<BreakType, DateTime>(BreakType.Additional, additional);

            return new Tuple<BreakType, DateTime>(BreakType.Main, main);
        }

        public void Notify(string message, int delay = 3000)
        {
            Notifier.ShowBalloonTip(delay, "Upcoming break", message, ToolTipIcon.Info);
        }

        public void Snooze()
        {
            _stateMachine.Fire(BreakTrigger.Snooze);
        }

        private void SnoozeStart()
        {
            EndOfSnooze = DateTime.Now.Add(Settings.SnoozeTime);
        }

        public TimeSpan? Tick()
        {
            if (_stateMachine.IsInState(BreakState.Waiting))
            {
                var next = NextBreak();
                if (next.Item1 == BreakType.None)
                    return null;

                var left = next.Item2 - DateTime.Now;
                if (left.TotalSeconds < 0)
                    BreakNow(next.Item1);
                else if (left.TotalSeconds <= 10)
                    _stateMachine.Fire(BreakTrigger.Alert10Seconds);
                else if (left.TotalMinutes <= 3)
                    _stateMachine.Fire(BreakTrigger.Alert3Minutes);

                return left;
            }

            if (_stateMachine.IsInState(BreakState.Breaking))
            {
                if (DateTime.Now > EndOfBreak)
                    _stateMachine.Fire(BreakTrigger.Completed);
            }
            else if (_stateMachine.IsInState(BreakState.Snoozing))
            {
                if (DateTime.Now > EndOfSnooze)
                    _stateMachine.Fire(BreakTrigger.Break);
            }

            return null;
        }

        public void LoadSettings()
        {
            var registry = Registry.CurrentUser.OpenSubKey(@"Software\Gefvert\BreakTime");
            if (registry == null)
                return;

            using (registry)
            {
                _settings.AdditionalBreakDone = (int)registry.GetValue("AdditionalBreakDone", 0) != 0;
                _settings.AdditionalBreakMinutesValue = (int)registry.GetValue("AdditionalBreakMinutes", 2);
                _settings.AllowClosing = (int)registry.GetValue("AllowClosing", 0) != 0;
                _settings.AllowSnoozing = (int)registry.GetValue("AllowSnoozing", 1) != 0;
                _settings.TimeStopValue = (int)registry.GetValue("HoursEnd", 8);
                _settings.TimeStartValue = (int)registry.GetValue("HoursStart", 18);
                _settings.LastBreakTime = DateTime.Parse((string)registry.GetValue("LastBreak", DateTime.Now.ToISO8601String()));
                _settings.MainBreakIntervalValue = (int)registry.GetValue("MainBreakInterval", 60);
                _settings.MainBreakMinutesValue = (int)registry.GetValue("MainBreakMinutes", 3);
                _settings.SnoozeTime = TimeSpan.FromMinutes((int)registry.GetValue("SnoozeTime", 5));
                _settings.UseAdditionalBreak = (int)registry.GetValue("UseAdditionalBreak", 0) != 0;
                _settings.UseHours = (int)registry.GetValue("UseHours", 1) != 0;
            }

            // Check if last break time is valid by checking the system boot time, as well as if the interval exceeds
            // the break time by 50%. If so, we assume the machine was turned off.
            var timeSinceLastBreak = DateTime.Now - _settings.LastBreakTime;
            var timeSinceLastBoot = TimeSpan.FromMilliseconds(WinApi.GetTickCount64());
            if (timeSinceLastBoot < timeSinceLastBreak || timeSinceLastBreak.TotalSeconds > _settings.MainBreakInterval.TotalSeconds * 1.5)
            {
                // The system was restarted between now and the last break. Reset the system.
                Reset();
            }
        }

        public void SaveSettings()
        {
            var registry = Registry.CurrentUser.CreateSubKey(@"Software\Gefvert\BreakTime");
            if (registry == null)
                throw new Exception("Unable to persist settings to registry.");

            using (registry)
            {
                registry.SetValue("AdditionalBreakDone", _settings.AdditionalBreakDone ? 1 : 0);
                registry.SetValue("AdditionalBreakMinutes", _settings.AdditionalBreakMinutesValue);
                registry.SetValue("AllowClosing", _settings.AllowClosing ? 1 : 0);
                registry.SetValue("AllowSnoozing", _settings.AllowSnoozing ? 1 : 0);
                registry.SetValue("HoursEnd", _settings.TimeStopValue);
                registry.SetValue("HoursStart", _settings.TimeStartValue);
                registry.SetValue("LastBreak", _settings.LastBreakTime.ToISO8601String());
                registry.SetValue("MainBreakInterval", _settings.MainBreakIntervalValue);
                registry.SetValue("MainBreakMinutes", _settings.MainBreakMinutesValue);
                registry.SetValue("SnoozeTime", (int)_settings.SnoozeTime.TotalMinutes);
                registry.SetValue("UseAdditionalBreak", _settings.UseAdditionalBreak ? 1 : 0);
                registry.SetValue("UseHours", _settings.UseHours ? 1 : 0);
            }
        }
    }
}
