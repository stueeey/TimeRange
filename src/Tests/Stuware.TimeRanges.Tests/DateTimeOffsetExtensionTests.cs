using FluentAssertions;
using Xunit;

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
        day.IsBetween((start, end), true).Should().BeTrue();
        day.IsBetween((start, end), false).Should().BeTrue();
    }
    
    [Fact]
    public void GivenADateTimeOffsetOutsideATimeRange_IsBetween_ShouldReturnFalse()
    {
        var start = LocalDay("2022/02/11");
        var end = LocalDay("2022/02/15");
        
        var before = LocalDay("2022/02/09");
        before.IsBetween((start, end)).Should().BeFalse();
        before.IsBetween((start, end), true).Should().BeFalse();
        before.IsBetween((start, end), false).Should().BeFalse();

        var after = LocalDay("2022/03/09");
        after.IsBetween((start, end)).Should().BeFalse();
        after.IsBetween((start, end), true).Should().BeFalse();
        after.IsBetween((start, end), false).Should().BeFalse();
    }
    
    [Fact]
    public void GivenADateTimeOffsetBorderingATimeRange_IsBetween_ShouldReturnFalseIfExclusiveAndTrueIfInclusive()
    {
        var start = LocalDay("2022/02/11").At("9:00 am");
        var end = LocalDay("2022/02/15").At("10:30 am");

        start.IsBetween((start, end)).Should().BeFalse();
        start.IsBetween((start, end), false).Should().BeFalse();
        start.IsBetween((start, end), true).Should().BeTrue();

        end.IsBetween((start, end)).Should().BeFalse();
        end.IsBetween((start, end), false).Should().BeFalse();
        end.IsBetween((start, end), true).Should().BeTrue();
    }
}