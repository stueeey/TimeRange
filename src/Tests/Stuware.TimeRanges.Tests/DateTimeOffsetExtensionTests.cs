using FluentAssertions;
using Xunit;
using static Stuware.TimeRanges.Tests.TestHelpers;

namespace Stuware.TimeRanges.Tests;

public class DateTimeOffsetExtensionTests
{
    [Fact]
    public void GivenADateTimeOffsetWithinATimeRange_IsBetween_ShouldReturnTrue()
    {
        var start = LocalDay("2022/02/11");
        var end = LocalDay("2022/02/15");
        
        var day = LocalDay("2022/02/13");

        day.IsBetween((start, end)).Should().BeTrue();
        day.IsBetweenInclusive((start, end)).Should().BeTrue();
        day.IsBetweenExclusive((start, end)).Should().BeTrue();
    }
    
    [Fact]
    public void GivenADateTimeOffsetOutsideATimeRange_IsBetween_ShouldReturnFalse()
    {
        var start = LocalDay("2022/02/11");
        var end = LocalDay("2022/02/15");
        
        var before = LocalDay("2022/02/09");
        before.IsBetween((start, end)).Should().BeFalse();
        before.IsBetweenInclusive((start, end)).Should().BeFalse();
        before.IsBetweenExclusive((start, end)).Should().BeFalse();
        
        var after = LocalDay("2022/03/09");
        after.IsBetween((start, end)).Should().BeFalse();
        after.IsBetweenInclusive((start, end)).Should().BeFalse();
        after.IsBetweenExclusive((start, end)).Should().BeFalse();
    }
    
    [Fact]
    public void GivenADateTimeOffsetBorderingATimeRange_IsBetween_ShouldReturnFalseIfExclusiveAndTrueIfInclusive()
    {
        var start = LocalDay("2022/02/11").At("9:00 am");
        var end = LocalDay("2022/02/15").At("10:30 am");

        start.IsBetween((start, end)).Should().BeFalse();
        start.IsBetween((start, end), false).Should().BeFalse();
        start.IsBetween((start, end), true).Should().BeTrue();
        start.IsBetweenInclusive((start, end)).Should().BeTrue();
        start.IsBetweenExclusive((start, end)).Should().BeFalse();
        
        end.IsBetween((start, end)).Should().BeFalse();
        end.IsBetween((start, end), false).Should().BeFalse();
        end.IsBetween((start, end), true).Should().BeTrue();
        end.IsBetweenInclusive((start, end)).Should().BeTrue();
        end.IsBetweenExclusive((start, end)).Should().BeFalse();
    }
}