namespace Stuware.TimeRanges;

public static class TimeSpanExtensions
{
    /// <summary>
    /// Returns a TimeRange that begins at <paramref name="start"/> and has a length specified by <paramref name="duration"/>
    /// </summary>
    /// <param name="duration">The desired length of the TimeRange</param>
    /// <param name="start">The start time of the TimeRange</param>
    /// <returns>A TimeRange starting at <paramref name="start"/> with a duration of <paramref name="duration"/></returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="duration"/> is less than or equal to <see cref="TimeSpan.Zero"/></exception>
    public static TimeRange StartingAt(this TimeSpan duration, DateTimeOffset start) =>
        duration <= TimeSpan.Zero
            ? throw new ArgumentException($"{duration} must be greater than zero", nameof(duration))
            : new TimeRange(start, start + duration);

    /// <summary>
    /// Returns a TimeRange ending at <paramref name="end"/> with a length specified by <paramref name="duration"/>
    /// </summary>
    /// <param name="duration">The desired length of the TimeRange</param>
    /// <param name="end">The end time of the TimeRange</param>
    /// <returns>A TimeRange ending at <paramref name="end"/> with a duration of <paramref name="duration"/></returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="duration"/> is less than or equal to <see cref="TimeSpan.Zero"/></exception>
    public static TimeRange EndingAt(this TimeSpan duration, DateTimeOffset end) =>
        duration <= TimeSpan.Zero
            ? throw new ArgumentException($"{duration} must be greater than zero", nameof(duration))
            : new TimeRange(end - duration, end);
}