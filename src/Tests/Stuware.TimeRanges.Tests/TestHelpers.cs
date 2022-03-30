using System;
using System.Globalization;

namespace Stuware.TimeRanges.Tests;

internal static class TestHelpers
{
    internal static DateTimeOffset UtcDay(string date) => Day(date, TimeZoneInfo.Utc);
    
    internal static DateTimeOffset LocalDay(string date) => Day(date, TimeZoneInfo.Local);
    internal static DateTimeOffset Day(string date, TimeZoneInfo timeZone)
    {
        var baseLocalDate = DateTime.Parse(date, CultureInfo.InvariantCulture).Date;
        return new DateTimeOffset(baseLocalDate, timeZone.GetUtcOffset(baseLocalDate));
    }

    internal static DateTimeOffset At(this DateTimeOffset baseDate, string time) => baseDate.Add(-baseDate.TimeOfDay).Add(DateTime.Parse(time).TimeOfDay);
}