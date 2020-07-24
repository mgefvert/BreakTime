using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace BreakTime.Classes
{
    public class OutlookMeetings
    {
        public class OutlookMeeting
        {
            public string Name { get; set; }
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
        }

        private readonly List<OutlookMeeting> _meetings = new List<OutlookMeeting>();

        public ICollection<OutlookMeeting> Meetings => _meetings;
        public DateTime LastUpdate { get; private set; }

        public void Update()
        {
            var app = new Outlook.Application();
            try
            {
                var calendar = app.Session.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderCalendar).Items;

                calendar.IncludeRecurrences = true;
                calendar.Sort("[Start]");
                calendar = calendar.Restrict(
                    $"[Start] >= '{DateTime.Today:yyyy-MM-dd}' AND [End] < '{DateTime.Today.AddDays(1):yyyy-MM-dd}'");

                _meetings.Clear();
                foreach (var item in calendar.Cast<Outlook.AppointmentItem>())
                {
                    _meetings.Add(new OutlookMeeting
                    {
                        Name = item.Subject,
                        Start = item.Start,
                        End = item.Start.AddMinutes(item.Duration)
                    });
                }

                LastUpdate = DateTime.Now;
            }
            catch (COMException)
            {
                //
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(app);
            }
        }

        public bool InMeeting()
        {
            var now = DateTime.Now;
            return _meetings.Any(m => m.Start.AddMinutes(-3) <= now && m.End.AddMinutes(5) >= now);
        }
    }
}
