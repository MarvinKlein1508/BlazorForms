using System.Resources;

namespace BlazorForms.Core.Extensions
{
    public static class DateTimeExtensions
    {
        private static readonly ResourceManager _ressourceManager = new ResourceManager("BlazorForms.Core.Languages.Extensions.DateTimeExtensions", typeof(DateTimeExtensions).Assembly);
        public static string GetElapsedTimeString(this DateTime yourDate)
        {

            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;

            var ts = new TimeSpan(DateTime.Now.Ticks - yourDate.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * MINUTE)
            {
                return GetLocalizedMessage(ts.Seconds, "SECONDS_AGO");
            }

            if (delta < 2 * MINUTE)
            {
                return GetLocalizedMessage("MINUTE_AGO");
            }

            if (delta < 45 * MINUTE)
            {
                return GetLocalizedMessage(ts.Minutes, "X_MINUTES_AGO");
            }

            if (delta < 120 * MINUTE)
            {
                return GetLocalizedMessage("HOUR_AGO");
            }

            if (delta < 24 * HOUR)
            {
                return GetLocalizedMessage(ts.Hours, "X_HOURS_AGO");
            }

            if (delta < 48 * HOUR)
            {
                return GetLocalizedMessage("YESTERDAY");
            }

            // Fallback to a default date format if none of the conditions are met
            return yourDate.ToString(GetLocalizedMessage("DEFAULT_DATE_FORMAT"));
        }

        private static string GetLocalizedMessage(string key)
        {
            return _ressourceManager.GetString(key) ?? key;
        }

        private static string GetLocalizedMessage(int count, string key)
        {
            var message = GetLocalizedMessage(key);

            return String.Format(message, count);
        }
    }


}
