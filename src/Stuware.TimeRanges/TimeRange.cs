namespace Stuware.TimeRanges;

public readonly struct TimeRange : IComparable, IComparable<TimeRange>, IComparable<DateTimeOffset>, IEquatable<TimeRange>
{
    public DateTimeOffset Start { get; }
    public DateTimeOffset End { get; }
    public TimeSpan Duration => End - Start;

    public TimeRange(DateTimeOffset start, DateTimeOffset end)
    {
        if (end > start)
        {
            Start = start;
            End = end;
        }
        else
        {
            Start = end;
            End = start;
        }
    }

    public TimeRange(TimeRange range)
    {
        Start = range.Start;
        End = range.End;
    }

#if !NETFRAMEWORK
    public static implicit operator TimeRange((DateTime start, DateTime end) range) => new(range.start, range.end);
    public static implicit operator TimeRange((DateTimeOffset start, DateTimeOffset end) range) => new(range.start, range.end);
    public static implicit operator (DateTime start, DateTime end)(TimeRange range) => (range.Start.LocalDateTime, range.End.LocalDateTime);
    public static implicit operator (DateTimeOffset start, DateTimeOffset end)(TimeRange range) => (range.Start, range.End);
#endif
    public static implicit operator TimeRange(Tuple<DateTime, DateTime> range) => new(range.Item1, range.Item2);
    public static implicit operator TimeRange(Tuple<DateTimeOffset, DateTimeOffset> range) => new(range.Item1, range.Item2);
    public static implicit operator Tuple<DateTime, DateTime>(TimeRange range) => new(range.Start.LocalDateTime, range.End.LocalDateTime);
    public static implicit operator Tuple<DateTimeOffset, DateTimeOffset>(TimeRange range) => new(range.Start, range.End);
    public static implicit operator TimeSpan(TimeRange range) => range.Duration;
    public static implicit operator DateTimeOffset(TimeRange range) => range.Start;
    public override string ToString() => ToString(true);

    public string ToString(bool includeStartDate, bool? includeEndDate = null, bool? includeTimeZone = null)
    {
        if (this.IsBlank())
            return "<Blank>";
        includeTimeZone ??= Start.Offset != End.Offset;
        includeEndDate ??= Start.DateTime.Date != End.DateTime.Date || (Start - End).Days != 0;
        return $"{DebuggerStrings.ToDebuggerString(Start, includeStartDate, includeTimeZone.Value)} - {DebuggerStrings.ToDebuggerString(End, includeEndDate.Value, includeTimeZone.Value)} [{DebuggerStrings.ToDebuggerString(Duration)}]";
    }

    #region Comparison
    public override bool Equals(object? obj)
    {
        if (obj is null) 
            return false;
        return obj is TimeRange range && Equals(range);
    }

    public int CompareTo(TimeRange other)
    {
        var startComparison = Start.CompareTo(other.Start);
        return startComparison != 0 
            ? startComparison 
            : End.CompareTo(other.End);
    }
    
    public int CompareTo(DateTimeOffset other)
    {
        if (Start.CompareTo(other) < 0)
            return -1;
        if (End.CompareTo(other) > 0)
            return 1; // 
        return 0; // Within time range
    }

    public int CompareTo(object? obj)
    {
        if (obj is null) 
            return 1;
        if (obj is TimeRange other) 
            return CompareTo(other);
        if (obj is DateTimeOffset otherDateTimeOffset) 
            return CompareTo(otherDateTimeOffset);
        throw new ArgumentException($"Object must be of type {nameof(TimeRange)} or {nameof(DateTimeOffset)}");
    }

    public bool Equals(TimeRange other) => Start.Equals(other.Start) && End.Equals(other.End);
    public override int GetHashCode()
    {
#if NET60
        return HashCode.Combine(Start, End);
#else
        // System.HashCode does not exist in .net standard or net40 :(
        unchecked
        {
            var hash = 17;
            hash = hash * 31 + Start.GetHashCode();
            hash = hash * 31 + End.GetHashCode();
            return hash;
        }
#endif
        
    }

    public static bool operator <(TimeRange left, TimeRange right) => Comparer<TimeRange>.Default.Compare(left, right) < 0;
    public static bool operator >(TimeRange left, TimeRange right) => Comparer<TimeRange>.Default.Compare(left, right) > 0;
    public static bool operator <(TimeRange left, DateTimeOffset right) => left.Start < right;
    public static bool operator >(TimeRange left, DateTimeOffset right) => left.End > right;
    public static bool operator <(DateTimeOffset left, TimeRange right) => left < right.Start;
    public static bool operator >(DateTimeOffset left, TimeRange right) => left > right.End;
    public static bool operator <=(TimeRange left, TimeRange right) => Comparer<TimeRange>.Default.Compare(left, right) <= 0;
    public static bool operator >=(TimeRange left, TimeRange right) => Comparer<TimeRange>.Default.Compare(left, right) >= 0;
    public static bool operator <=(TimeRange left, DateTimeOffset right) => left.CompareTo(right) <= 0;
    public static bool operator >=(TimeRange left, DateTimeOffset right) => left.CompareTo(right) >= 0;
    public static bool operator <=(DateTimeOffset? left, TimeRange? right) => right?.CompareTo(left) >= 0;
    public static bool operator >=(DateTimeOffset? left, TimeRange? right) => right?.CompareTo(left) <= 0;
    public static bool operator ==(TimeRange left, TimeRange right) => Equals(left, right);
    public static bool operator !=(TimeRange left, TimeRange right) => !Equals(left, right);
    public static bool operator ==(DateTimeOffset left, TimeRange right) => right.CompareTo(left) == 0;
    public static bool operator !=(DateTimeOffset left, TimeRange right) => right.CompareTo(left) != 0;
    public static TimeRange operator +(TimeRange left, TimeSpan right) => new(left.Start.Add(right), left.End.Add(right));
    public static TimeRange operator -(TimeRange left, TimeSpan right) => new(left.Start.Add(-right), left.End.Add(-right));
    public static IEnumerable<TimeRange> operator /(TimeRange left, int parts) => left.SplitInto(parts);
    public static IEnumerable<TimeRange> operator /(TimeRange left, TimeSpan duration) => left.SplitInto(duration);
    public static TimeRange operator &(TimeRange left, TimeRange overlappingTimeRange) => left.GetOverlap(overlappingTimeRange);
    public static IEnumerable<TimeRange> operator &(TimeRange left, IEnumerable<TimeRange> overlappingTimeRanges) => new[] {left}.Overlapping(overlappingTimeRanges);
    public static IEnumerable<TimeRange> operator ^(TimeRange left, TimeRange overlappingTimeRange) => left.Excluding(overlappingTimeRange);
    public static IEnumerable<TimeRange> operator ^(TimeRange left, IEnumerable<TimeRange> exclusions) => left.Excluding(exclusions);
    #endregion
}