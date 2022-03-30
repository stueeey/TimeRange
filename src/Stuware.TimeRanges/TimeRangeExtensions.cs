﻿using static Stuware.TimeRanges.DateTimeOffsetExtensions;

namespace Stuware.TimeRanges;

public static class TimeRangeExtensions
{
    /// <summary>
    /// Returns true if the given ranges overlap each other
    /// </summary>
    /// <param name="rangeA">The first range</param>
    /// <param name="rangeB">The second range</param>
    /// <param name="inclusive">If true, time ranges that share a start or end will be treated as overlapping. Defaults to false</param>
    /// <returns>True if the ranges intersect</returns>
    public static bool Overlaps(this TimeRange rangeA, TimeRange rangeB, bool inclusive = false) =>
        rangeA == rangeB ||
        rangeA.Start.IsBetween(rangeB, inclusive) || 
        rangeA.End.IsBetween(rangeB, inclusive) ||
        rangeB.Start.IsBetween(rangeA, inclusive) || 
        rangeB.End.IsBetween(rangeA, inclusive);

    /// <summary>
    /// Returns true if the given ranges overlap each other within a margin of error
    /// </summary>
    /// <param name="rangeA">The first range</param>
    /// <param name="rangeB">The second range</param>
    /// <param name="margin">The margin of error</param>
    /// <returns>True if the ranges intersect</returns>
    /// <remarks>If you supply two time ranges starting and ending 1 min apart and a margin of 1 minute then the function will consider them to be overlapping</remarks>
    public static bool Overlaps(this TimeRange rangeA, TimeRange rangeB, TimeSpan margin, bool inclusive = false)
    {
        if (AnyBlank(rangeA, rangeB))
            return false;
        if (margin == default)
            return Overlaps(rangeA, rangeB);
        return rangeA.Start.Add(-margin).IsBetween(rangeB, inclusive) || 
               rangeA.End.Add(margin).IsBetween(rangeB, inclusive) ||
               rangeB.Start.Add(-margin).IsBetween(rangeA, inclusive) || 
               rangeB.End.Add(margin).IsBetween(rangeA, inclusive);
    }
    
    internal static bool AnyBlank(params TimeRange[] ranges) => ranges.Any(r => r.IsBlank());
    
    /// <summary>
    /// Returns true if the supplied TimeRange is null or blank
    /// </summary>
    /// <param name="range">The TimeRange to check</param>
    /// <returns>True if the TimeRange is blank or null</returns>
    public static bool IsBlank(this TimeRange? range) => range is null || range.Value.IsBlank();
    
    /// <summary>
    /// Returns true if the supplied TimeRange is blank
    /// </summary>
    /// <param name="range">The TimeRange to check</param>
    /// <returns>True if the TimeRange is blank</returns>
    public static bool IsBlank(this TimeRange range) => range.Start == DateTimeOffset.MinValue && range.End == DateTimeOffset.MinValue;
    
    /// <summary>
    /// Returns a TimeRange representing the overlap between two TimeRanges, or null if there is no overlap
    /// </summary>
    /// <param name="rangeA">The first range</param>
    /// <param name="rangeB">The second range</param>
    public static TimeRange? GetOverlap(this TimeRange rangeA, TimeRange rangeB) =>
        !Overlaps(rangeA, rangeB) 
            ? null 
            : (TimeRange?)new TimeRange(Max(rangeA.Start, rangeB.Start), Min(rangeA.End, rangeB.End));
}