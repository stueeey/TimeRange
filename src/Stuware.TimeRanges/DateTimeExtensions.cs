namespace Stuware.TimeRanges;

public static class DateTimeExtensions
{

    #region Internal

    internal static DateTime EndOfDay(this DateTime date) => date.AtTimeOfDay(TimeSpan.FromDays(1) + TimeSpan.FromMilliseconds(-1));
    internal static DateTime AtTimeOfDay(this DateTime date, TimeSpan timeOfDay) => date.Date.Add(timeOfDay);
    internal static DateTime Max(params DateTime[] dateTimes) => dateTimes.Max();
    internal static DateTime Min(params DateTime[] dateTimes) => dateTimes.Min();

    #endregion
}