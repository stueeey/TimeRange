namespace Stuware.TimeRanges;

public static class DateTimeOffsetExtensions
{
    /// <summary>
    /// Returns true if the given candidate DateTimeOffset falls within the given TimeRange (exclusive by default)
    /// </summary>
    /// <param name="candidate">The timestamp to consider</param>
    /// <param name="timeRange">The time range to check if the candidate timestamp is between</param>
    /// <param name="inclusive">True if the candidate should be treated as between if it falls exactly on the start or end</param>
    /// <returns>True if the candidate falls within timeRange</returns>
    public static bool IsBetween(this DateTimeOffset candidate, TimeRange timeRange, bool inclusive = false) => inclusive 
        ? candidate.IsBetweenInclusive(timeRange)
        : candidate.IsBetweenExclusive(timeRange);
    
    /// <summary>
    /// Returns true if the given candidate DateTimeOffset falls within the given TimeRange (inclusive)
    /// </summary>
    /// <param name="candidate">The timestamp to consider</param>
    /// <param name="timeRange">The time range to check if the candidate timestamp is between</param>
    /// <returns>True if the candidate falls within timeRange</returns>
    public static bool IsBetweenInclusive(this DateTimeOffset candidate, TimeRange timeRange) => candidate >= timeRange.Start && candidate <= timeRange.End;
    
    /// <summary>
    /// Returns true if the given candidate DateTimeOffset falls within the given TimeRange (exclusive)
    /// </summary>
    /// <param name="candidate">The timestamp to consider</param>
    /// <param name="timeRange">The time range to check if the candidate timestamp is between</param>
    /// <returns>True if the candidate falls within timeRange</returns>
    public static bool IsBetweenExclusive(this DateTimeOffset candidate, TimeRange timeRange) => candidate > timeRange.Start && candidate < timeRange.End;

    #region Internal
    
    internal static DateTimeOffset Max(params DateTimeOffset[] dateTimes) => dateTimes.Max();
    internal static DateTimeOffset Min(params DateTimeOffset[] dateTimes) => dateTimes.Min();
    
    #endregion
}