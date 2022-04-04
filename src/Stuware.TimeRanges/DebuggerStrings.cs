using System.Text;

namespace Stuware.TimeRanges;

internal static class DebuggerStrings
{
    internal static string ToUnambiguousDateString(this DateTime dateTime)
    {
        return dateTime.ToString("dd MMM yyyy");
    }
    
    internal static string ToDebuggerString(DateTimeOffset dateTime, bool includeDate, bool includeTimezone)
    {
        var stringBuilder = new StringBuilder();
        if (dateTime.Offset == TimeSpan.Zero)
        {
            if (includeDate)
            {
                stringBuilder.Append(dateTime.UtcDateTime.ToUnambiguousDateString());
                stringBuilder.Append(' ');
            }
            stringBuilder.Append(dateTime.UtcDateTime.ToShortTimeString());
            if (includeTimezone)
                stringBuilder.Append(" (UTC)");
            return stringBuilder.ToString();
        }
        if (includeDate)
        {
            stringBuilder.Append(dateTime.LocalDateTime.ToUnambiguousDateString());
            stringBuilder.Append(' ');
        }
        stringBuilder.Append(dateTime.LocalDateTime.ToShortTimeString());
        if (includeTimezone)
        {
            stringBuilder.Append(" (+");
            stringBuilder.Append(ToDebuggerString(dateTime.Offset));
            stringBuilder.Append(')');
        }
        return stringBuilder.ToString();
    }
    
    internal static string ToDebuggerString(TimeSpan duration, int maxParts = 2)
    {
        if (duration == TimeSpan.Zero)
            return "Zero";
        maxParts = Math.Max(1, maxParts);
        var returnedParts = 0;
        var builder = new StringBuilder();
        if (duration >= TimeSpan.FromDays(7))
        {
            builder.Append(duration.Days);
            builder.Append(" w ");
            duration -= TimeSpan.FromDays(duration.Days * 7);
            returnedParts++;
            if (returnedParts >= maxParts)
                return builder.ToString().TrimEnd();
        }
        if (duration >= TimeSpan.FromDays(1) && duration.Days != 0)
        {
            builder.Append(duration.Days);
            builder.Append(" d ");
            duration -= TimeSpan.FromDays(duration.Days);
            returnedParts++;
            if (returnedParts >= maxParts)
                return builder.ToString().TrimEnd();
        }
        if (duration >= TimeSpan.FromHours(1) && duration.Hours != 0)
        {
            builder.Append(duration.Hours);
            builder.Append(" h ");
            duration -= TimeSpan.FromHours(duration.Hours);
            returnedParts++;
            if (returnedParts >= maxParts)
                return builder.ToString().TrimEnd();
        }
        if (duration >= TimeSpan.FromMinutes(1) && duration.Minutes != 0)
        {
            builder.Append(duration.Minutes);
            builder.Append(" m ");
            duration -= TimeSpan.FromMinutes(duration.Minutes);
            returnedParts++;
            if (returnedParts >= maxParts)
                return builder.ToString().TrimEnd();
        }
        if (duration >= TimeSpan.FromSeconds(1) && duration.Seconds != 0)
        {
            builder.Append(duration.Seconds);
            builder.Append(" s ");
            duration -= TimeSpan.FromSeconds(duration.Seconds);
            returnedParts++;
            if (returnedParts >= maxParts)
                return builder.ToString().TrimEnd();
        }
        if (duration >= TimeSpan.FromMilliseconds(1) && duration.Milliseconds != 0)
        {
            builder.Append(duration.Milliseconds);
            builder.Append(" ms ");
            duration -= TimeSpan.FromMilliseconds(duration.Milliseconds);
            returnedParts++;
            if (returnedParts >= maxParts)
                return builder.ToString().TrimEnd();
        }
        if (duration >= TimeSpan.FromTicks(1) && duration.Ticks != 0)
        {
            builder.Append(duration.Ticks);
            builder.Append(" ticks");
        }
        return builder.ToString().TrimEnd();   
    }
}