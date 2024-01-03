using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static string RelativeZeit(this DateTime yourDate)
        {
            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;

            var ts = new TimeSpan(DateTime.Now.Ticks - yourDate.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * MINUTE)
            {
                return ts.Seconds == 1 ? "vor einer Sekunde" : $"vor {ts.Seconds} Sekunden";
            }

            if (delta < 2 * MINUTE)
            {
                return "vor einer Minute";
            }

            if (delta < 45 * MINUTE)
            {
                return $"vor {ts.Minutes} Minuten";
            }

            if (delta < 120 * MINUTE)
            {
                return "vor einer Stunde";
            }

            if (delta < 24 * HOUR)
            {
                return $"vor {ts.Hours} Stunden";
            }

            if (delta < 48 * HOUR)
            {
                return "gestern";
            }

            return DateTime.Now.ToString("dd. MMMM yyyy");
        }
    }
}
