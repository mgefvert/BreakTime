using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DotNetCommons.WinForms;

namespace BreakTime.Classes
{
    public class Movement
    {
        private const int KeyboardIncrease = 5;
        private const int MouseIncrease = 1;

        public const int MinutesPerCounter = 5;

        private int _counter;
        private int _keyboardState;
        private int _mouseState;
        private readonly Dictionary<long, int> _history = new Dictionary<long, int>();

        public int Counter => _counter;
        public IReadOnlyDictionary<long, int> History => _history;

        private static int GetKeyState()
        {
            var result = 0;
            foreach (int key in Enum.GetValues(typeof(Keys)))
            {
                if (WinApi.GetAsyncKeyState(key) != 0)
                    result += key;
            }

            return result;
        }

        private static int GetMouseState()
        {
            var pos = Cursor.Position;
            return pos.X ^ pos.Y;
        }

        public void CheckMotion()
        {
            var increase = 0;

            var n = GetKeyState();
            if (_keyboardState != n)
            {
                _keyboardState = n;
                increase += KeyboardIncrease;
            }

            n = GetMouseState();
            if (_mouseState != n)
            {
                _mouseState = n;
                increase += MouseIncrease;
            }

            _counter += increase;

            var historyKey = DateTime.Now.Ticks / (MinutesPerCounter * TimeSpan.TicksPerMinute);
            if (!_history.ContainsKey(historyKey))
                _history[historyKey] = 0;
            
            _history[historyKey] += increase;
        }

        public List<int> ExtractMotionCounters()
        {
            var keys = _history.Keys.OrderByDescending(x => x).Skip(1).ToList();
            var result = keys.Select(key => _history[key]).ToList();
            keys.ForEach(key => _history.Remove(key));

            return result;
        }

        public void Reset()
        {
            _counter = 0;
        }
    }
}
