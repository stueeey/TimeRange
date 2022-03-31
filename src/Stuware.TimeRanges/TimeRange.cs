namespace Stuware.TimeRanges;

public readonly struct TimeRange : IComparable, IComparable<TimeRange>, IComparable<DateTimeOffset>, IEquatable<TimeRange>
{
    public TimeRange(DateTime start, DateTime end)
    {
        Start = start;
        End = end;
    }
    
    public TimeRange(DateTimeOffset start, DateTimeOffset end)
    {
        Start = start;
        End = end;
    }

    public TimeRange(TimeRange range)
    {
        Start = range.Start;
        End = range.End;
    }

    public DateTimeOffset Start { get; }
    public DateTimeOffset End { get; }

    public TimeSpan Duration => End - Start;

#if !NET46
    public static implicit operator TimeRange((DateTime start, DateTime end) range) => new(range.start, range.end);
    public static implicit operator TimeRange((DateTimeOffset start, DateTimeOffset end) range) => new(range.start, range.end);
    public static implicit operator (DateTime start, DateTime end)(TimeRange range) => (range.Start.LocalDateTime, range.End.LocalDateTime);
    public static implicit operator (DateTimeOffset start, DateTimeOffset end)(TimeRange range) => (range.Start, range.End);
#endif
    public static implicit operator TimeRange(Tuple<DateTime, DateTime> range) => new(range.Item1, range.Item2);
    public static implicit operator TimeRange(Tuple<DateTimeOffset, DateTimeOffset> range) => new(range.Item1, range.Item2);
    public static implicit operator Tuple<DateTime, DateTime>(TimeRange range) => new(range.Start.LocalDateTime, range.End.LocalDateTime);
    public static implicit operator Tuple<DateTimeOffset, DateTimeOffset>(TimeRange range) => new(range.Start, range.End);
    
    public override string ToString()
    {
        if (this.IsBlank())
            return $"<Blank {nameof(TimeRange)}>";
        // TODO - timezones
        return Start.Date == End.Date
            ? $"{Start.LocalDateTime.ToShortTimeString()} - {End.LocalDateTime.ToShortTimeString()} ({Duration.Hours} h {Duration.Minutes} m)"
            : $"{Start} - {End} ({Duration.Hours} h {Duration.Minutes} m)";
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
    public override int GetHashCode() => HashCode.Combine(Start, End);
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

    #endregion
}