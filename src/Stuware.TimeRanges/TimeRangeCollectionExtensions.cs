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
        return ConsolidateSortedTimeRanges(items, margin);
    }

    // ReSharper disable once CognitiveComplexity
    /// <summary>
    ///     Consolidates a pre-sorted list of TimeRanges, merging any that overlap
    /// </summary>
    /// <param name="sortedTimeRanges">The sorted time ranges to consolidate</param>
    /// <param name="margin">The margin to use when finding overlaps - e.g. a margin of 5 minutes would merge any Time Ranges less than 5 minutes apart</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="sortedTimeRanges"/> is not sorted according to the default sort order for a TimeRange</exception>
    /// <returns>The merged time ranges</returns>
    public static IEnumerable<TimeRange> ConsolidateSortedTimeRanges(IReadOnlyList<TimeRange> sortedTimeRanges,
        TimeSpan margin = default)
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
                throw new ArgumentException(
                    $"{nameof(ConsolidateSortedTimeRanges)} expects {sortedTimeRanges} to be sorted. Either sort the list before passing it in or use {nameof(Consolidate)}");
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

    public static IEnumerable<TimeRange> Excluding(this IEnumerable<TimeRange> ranges, IEnumerable<TimeRange> exclusions) => ExcludingSortedTimeRanges(SortAndConsolidateTimeRanges(ranges), SortAndConsolidateTimeRanges(exclusions));
    
    public static IEnumerable<TimeRange> ExcludingSortedTimeRanges(IReadOnlyList<TimeRange> sortedTimeRanges,
        IReadOnlyList<TimeRange> sortedExclusions)
    {
        sortedTimeRanges = ConsolidateSortedTimeRanges(sortedTimeRanges).ToList();
        return ConsolidateSortedTimeRanges(sortedExclusions)
            .SelectMany(r => ExcludingSortedTimeRanges(r, sortedTimeRanges));
    }

    public static IEnumerable<TimeRange> Excluding(this TimeRange range, IEnumerable<TimeRange> exclusions) => 
        ExcludingSortedTimeRanges(range, SortAndConsolidateTimeRanges(exclusions));

    private static IReadOnlyList<TimeRange> SortAndConsolidateTimeRanges(IEnumerable<TimeRange> timeRanges)
    {
        // This will internally sort before consolidation. Consolidate is guaranteed to return 
        // TimeRanges in sorted order
        return timeRanges.Consolidate().ToList();
    }

    // ReSharper disable once CognitiveComplexity - It's as simple as it can performantly be
    public static IEnumerable<TimeRange> ExcludingSortedTimeRanges(TimeRange range, IReadOnlyList<TimeRange> sortedExclusions)
    {
        if (sortedExclusions.Count == 0)
        {
            yield return range;
            yield break;
        }

        var currentBlockStart = range.Start;
        TimeRange lastExclusion = default;
        foreach (var exclusion in sortedExclusions)
        {
            if (exclusion < lastExclusion)
                throw new ArgumentException($"{nameof(ExcludingSortedTimeRanges)} expects {sortedExclusions} to be sorted. Either sort the list before passing it in or use {nameof(Excluding)}");
            lastExclusion = exclusion;
            if (range.End <= exclusion.Start)
                break; // The exclusions are sorted so we can assume there are no more interesting ones
            if (!range.Overlaps(exclusion))
                continue; // Not relevant

            if (currentBlockStart >= range.End)
                yield break;
            if (exclusion.Start > currentBlockStart)
            {
                yield return new TimeRange(currentBlockStart, exclusion.Start);
                currentBlockStart = exclusion.End;
            }
            else
                currentBlockStart = Max(exclusion.End, currentBlockStart);
        }

        if (currentBlockStart < range.End)
            yield return new TimeRange(currentBlockStart, range.End);
    }

    public static IEnumerable<TimeRange> Overlapping(this IEnumerable<TimeRange> rangesA, IEnumerable<TimeRange> rangesB) => 
        Consolidate(GetOverlapOfSortedTimeRanges(SortAndConsolidateTimeRanges(rangesA), SortAndConsolidateTimeRanges(rangesB)));
    
    // ReSharper disable once CognitiveComplexity - It's as simple as it can performantly be
    // ReSharper disable once MemberCanBePrivate.Global - Exposing so it can be used in situations where performance is important
    public static IEnumerable<TimeRange> GetOverlapOfSortedTimeRanges(IReadOnlyList<TimeRange> sortedRangesA, IReadOnlyList<TimeRange> sortedRangesB)
    {
        if (sortedRangesA.Count == 0 || sortedRangesB.Count == 0)
            yield break;
        var indexA = 1;
        var indexB = 1;

        var currentA = sortedRangesA[0];
        var currentB = sortedRangesB[0];

        while (true)
        {
            var overlap = currentA.GetOverlap(currentB);
            if (overlap.IsBlank())
                yield return overlap;

            // Get the next entry on whichever 
            if (currentA.End >= currentB.End)
            {
                if (sortedRangesB.Count <= indexB)
                    yield break;
                var previousB = currentB;
                currentB = sortedRangesB[indexB];
                if (previousB > currentB)
                    throw new ArgumentException("sortedRangesB must be sorted according the the default TimeRange sort order", nameof(sortedRangesB));
                indexB++;
                continue;
            }
            if (sortedRangesA.Count <= indexA)
                yield break;
            var previousA = currentA;
            currentA = sortedRangesA[indexA];
            if (previousA > currentA)
                throw new ArgumentException("sortedRangesA must be sorted according the the default TimeRange sort order", nameof(sortedRangesA));
            indexA++;
        }
    }

    public static IEnumerable<TimeRange> SplitIntoDays(this TimeRange timeRange)
    {
        for (var current = timeRange.Start.Date; current <= timeRange.End.Date; current = current.AddDays(1))
            if (timeRange.Start.Date == current)
                yield return new TimeRange(timeRange.Start, current.EndOfDay());
            else if (timeRange.End.Date == current)
                yield return new TimeRange(current, timeRange.End);
            else
                yield return new TimeRange(current, current.EndOfDay());
    }
}