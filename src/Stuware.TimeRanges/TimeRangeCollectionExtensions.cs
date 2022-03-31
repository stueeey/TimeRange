using static Stuware.TimeRanges.DateTimeOffsetExtensions;

namespace Stuware.TimeRanges;

public static class TimeRangeCollectionExtensions
{
    private static TimeRange Accumulate(params TimeRange[] ranges)
    {
        var start = ranges
            .Where(r => !r.IsBlank())
            .Select(r => r.Start)
            .Min();
        var end = ranges
            .Where(r => !r.IsBlank())
            .Select(r => r.End)
            .Max();
        return new TimeRange(start, end);
    }
    
    /// <summary>
    ///     Consolidates a list of TimeRanges, merging any that overlap
    /// </summary>
    /// <param name="timeRanges">The time ranges to consolidate</param>
    /// <param name="margin">The margin to use when finding overlaps - e.g. a margin of 5 minutes would merge any Time Ranges less than 5 minutes apart</param>
    /// <returns>The merged time ranges</returns>
    /// <remarks>Assumes that the list is unsorted - if you know that your list is already sorted then use ConsolidateSortedTimeRanges</remarks>
    public static IEnumerable<TimeRange> Consolidate(this IEnumerable<TimeRange> timeRanges, TimeSpan margin = default)
    {
        var items = new List<TimeRange>(timeRanges);
        items.Sort();
        return items.ConsolidateSortedTimeRanges(margin);
    }

    // ReSharper disable once CognitiveComplexity
    /// <summary>
    ///     Consolidates a pre-sorted list of TimeRanges, merging any that overlap
    /// </summary>
    /// <param name="sortedTimeRanges">The sorted time ranges to consolidate</param>
    /// <param name="margin">The margin to use when finding overlaps - e.g. a margin of 5 minutes would merge any Time Ranges less than 5 minutes apart</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="sortedTimeRanges"/> is not sorted according to the default sort order for a TimeRange</exception>
    /// <returns>The merged time ranges</returns>
    public static IEnumerable<TimeRange> ConsolidateSortedTimeRanges(this IReadOnlyList<TimeRange> sortedTimeRanges, TimeSpan margin = default)
    {
        switch (sortedTimeRanges.Count)
        {
            case 0:
                yield break;
            case 1:
                yield return sortedTimeRanges[0];
                yield break;
        }

        TimeRange accumulatedTimeRange = default;
        
        var current = sortedTimeRanges[1];
        var previous = sortedTimeRanges[0];
        var index = 2;

        TimeRange GetConsolidatedTimeRange()
        {
            if (accumulatedTimeRange.IsBlank()) 
                return previous; // Not overlapping, return as-is
            
            // This is the end of a contiguous block, return it
            var cumulativeTimeRange = accumulatedTimeRange;
            accumulatedTimeRange = default; 
            return cumulativeTimeRange;
        }

        while (true)
        {
            if (previous > current)
                throw new ArgumentException($"{nameof(ConsolidateSortedTimeRanges)} expects {sortedTimeRanges} to be sorted. Either sort the list before passing it in or use {nameof(Consolidate)}");
            if (previous.Overlaps(current, margin, true))
                accumulatedTimeRange = Accumulate(accumulatedTimeRange, previous, current);
            else
                yield return GetConsolidatedTimeRange();
            
            previous = current;
            if (index != sortedTimeRanges.Count)
            {
                current = sortedTimeRanges[index];
                index++;
                continue;
            }

            // Return the last range / accumulated range
            yield return GetConsolidatedTimeRange();
            yield break;
        }
    }
}