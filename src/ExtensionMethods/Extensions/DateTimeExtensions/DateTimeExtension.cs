using System;
using Extensions.GenericExtensions;

namespace Extensions.DateTimeExtensions {
    public static class DateTimeExtension {
        public static bool Between(this DateTime obj, DateTime startDate, DateTime endDate) {
            return obj.Ticks.Between(startDate.Ticks, endDate.Ticks);
        }
        public static DateTime SetTime(this DateTime date, int hour, int min) {
            var dt = Convert.ToDateTime(date.ToShortDateString());
            var time = new TimeSpan(hour, min, 0);
            dt = dt.Add(time);
            return dt;
        }
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek) {
            var diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0) {
                diff += 7;
            }

            return dt.AddDays(-1 * diff).Date;
        }
    }
}
