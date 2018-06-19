using System;
using System.Collections.Generic;

namespace TestLibraryHotel

{
    public enum DateTimeComponents
    {
        Year,
        Month,
        Day,
        Hour,
        Minute,
        Second
    }

    public static class DateTimeHelper
    {
        public static DateTime Today
        {
            get
            {
                var today = DateTime.Today;
                return new DateTime(today.Year, today.Month, today.Day);
            }
        }

        public static List<DateTime> GetStartAndEndDates(int daysFromToday, int noOfDays)
        {
            return new List<DateTime> { Today.AddDays(daysFromToday), Today.AddDays(daysFromToday + noOfDays) };
        }
    }
}