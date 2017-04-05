using System;
using System.Linq;
using System.Windows.Forms;
using Stateless;

namespace BreakTime
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
        public BreakSettings Settings { get; set; } = new BreakSettings();

        public DateTime EndOfBreak { get; private set; }
        public DateTime EndOfSnooze { get; private set; }
        private DateTime _lastBreakTime = DateTime.Now.AddMinutes(-60).AddSeconds(5);
        private bool _additionalBreakDone = false;

        public NotifyIcon Notifier { get; set; }
        public Form BreakForm { get; set; }
        public bool SnoozeAllowed => _stateMachine.PermittedTriggers.Any(x => x == BreakTrigger.Snooze);

        public BreakController()
        {
            _stateMachine = new StateMachine<BreakState, BreakTrigger>(BreakState.Waiting);

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
                .OnEntry(() => Notify("A break is imminent. Stop working."))
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
                .OnEntry(SnoozeStart);

            _stateMachine.Configure(BreakState.Snoozing1)
                .SubstateOf(BreakState.Snoozing)
                .Permit(BreakTrigger.Break, BreakState.BreakingAfterSnoozing1);

            _stateMachine.Configure(BreakState.Snoozing2)
                .SubstateOf(BreakState.Snoozing)
                .Permit(BreakTrigger.Break, BreakState.BreakingAfterSnoozing2);
        }

        private void SnoozeStart()
        {
            EndOfSnooze = DateTime.Now.Add(Settings.SnoozeTime);
        }

        private void BreakEnd()
        {
            BreakForm.Hide();
        }

        private void BreakStart()
        {
            EndOfBreak = DateTime.Now.Add(Settings.MainBreakMinutes);
            BreakForm.Show();
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

            var main = _lastBreakTime.Add(Settings.MainBreakInterval);
            var additional = Settings.UseAdditionalBreak && !_additionalBreakDone
                ? _lastBreakTime.AddSeconds(Settings.MainBreakInterval.TotalSeconds / 2)
                : DateTime.MaxValue;

            if (additional < main)
                return new Tuple<BreakType, DateTime>(BreakType.Additional, additional);

            return new Tuple<BreakType, DateTime>(BreakType.Main, main);
        }

        public void Notify(string message)
        {
            Notifier.ShowBalloonTip(3000, "Upcoming break", message, ToolTipIcon.Info);
        }

        public void Tick()
        {
            if (_stateMachine.IsInState(BreakState.Waiting))
            {
                var next = NextBreak();
                if (next.Item1 == BreakType.None)
                    return;

                var left = next.Item2 - DateTime.Now;
                if (left.TotalSeconds < 0)
                    _stateMachine.Fire(BreakTrigger.Break);
                else if (left.TotalSeconds <= 10)
                    _stateMachine.Fire(BreakTrigger.Alert10Seconds);
                else if (left.TotalMinutes <= 3)
                    _stateMachine.Fire(BreakTrigger.Alert3Minutes);
            }
            else if (_stateMachine.IsInState(BreakState.Breaking))
            {
                if (DateTime.Now > EndOfBreak)
                    _stateMachine.Fire(BreakTrigger.Completed);
            }
            else if (_stateMachine.IsInState(BreakState.Snoozing))
            {
                if (DateTime.Now > EndOfSnooze)
                    _stateMachine.Fire(BreakTrigger.Break);
            }
        }

        public void Snooze()
        {
            _stateMachine.Fire(BreakTrigger.Snooze);
        }
    }
}
