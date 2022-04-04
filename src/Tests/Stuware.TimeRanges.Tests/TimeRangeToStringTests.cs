using System;
using FluentAssertions;
using Xunit;

namespace Stuware.TimeRanges.Tests;

public class TimeRangeToStringTests
{
    [Fact]
    public void GivenABlankTimeRange_ToString_ShouldResultInStringSayingBlank()
    {
        TimeRange range = default;
        range.ToString().Should().Be("<Blank>");
    }
    
    [Fact]
    public void GivenATimeRangeWhichStartsAndEndsOnTheSameDay_ToString_ShouldResultInStringWithOneDateTwoTimesAndNoTimeZones()
    {
        var day = Day("2022/02/13", TimeSpan.FromHours(13));
        TimeRange range = (day.At("10 am"), day.At("11:34 pm"));
        range.ToString().Should().Be("13 Feb 2022 10:00 am - 11:34 pm [13 h 34 m]");
    }
    
    [Fact]
    public void GivenATimeRangeWhichStartsAndEndsOnDifferentDays_ToString_ShouldResultInStringWithTwoDatesTwoTimesAndNoTimeZones()
    {
        var day = Day("2022/02/13", TimeSpan.FromHours(13));
        TimeRange range = (day.At("9 am"), day.AddDays(1).At("11:34 pm"));
        range.ToString().Should().Be("13 Feb 2022 9:00 am - 14 Feb 2022 11:34 pm [1 d 14 h]");
    }
    
    [Fact]
    public void GivenATimeRangeWhichStartsAndEndsWithDifferentDaysInDifferentTimeZones_ToString_ShouldResultInStringWithTwoDatesTwoTimesAndExplicitTimeZones()
    {
        var day = Day("2022/02/13", TimeSpan.FromHours(13));
        var dayUtc = day.ToOffset(TimeSpan.Zero);
        TimeRange range = (day.At("9 am"), dayUtc.AddDays(1).At("11:34 pm"));
        range.ToString().Should().Be("13 Feb 2022 9:00 am (+13 h) - 13 Feb 2022 11:34 pm (UTC) [1 d 3 h]");
    }
    
}