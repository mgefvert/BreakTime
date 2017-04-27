using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DotNetCommons;

namespace BreakTime.Classes
{
    public class Fortunes
    {
        public List<string> List { get; } = new List<string>();
        private readonly Random _rnd = new Random();
        private readonly List<int> _order = new List<int>();

        public Fortunes()
        {
        }

        public Fortunes(string filename)
        {
            Load(filename);
        }

        private void Load(string filename)
        {
            if (!File.Exists(filename))
                return;

            var text = File.ReadAllText(filename).Replace("\r", "");
            var fortunes = text.Split(new[] { "\n%\n" }, StringSplitOptions.None)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x));

            List.AddRange(fortunes);
        }

        public string Random()
        {
            if (!List.Any())
                return null;

            if (!_order.Any())
                _order.AddRange(Enumerable.Range(0, List.Count));

            var n = _order.ExtractAt(_rnd.Next(_order.Count));
            return List[n];
        }
    }
}
