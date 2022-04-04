using System;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Stuware.TimeRanges.Tests;

public class TimeRangeExtensionTests
{
    [Fact]
    public void GivenABlankTimeRange_IsBlank_ShouldReturnTrue()
    {
        TimeRange blank = default;
        blank.IsBlank().Should().BeTrue();
        new TimeRange().IsBlank().Should().BeTrue();
        ((TimeRange?)null).IsBlank().Should().BeTrue();
    }
    
    [Fact]
    public void GivenANonBlankTimeRange_IsBlank_ShouldReturnFalse()
    {
        var day = LocalDay("2022/02/13");
        TimeRange range1 = (day.At("10 am"), day.At("11:34 pm"));

        range1.IsBlank().Should().BeFalse();
    }
    
    [Fact]
    public void GivenNonIntersectingTimeRanges_Overlaps_ShouldReturnFalse()
    {
        var day = LocalDay("2022/01/10");
        TimeRange range1 = (day.At("10 am"), day.At("11:30 am"));
        TimeRange range2 = (day.At("1 pm"), day.At("1:30 pm"));

        range1.Overlaps(range2).Should().BeFalse();
    }
    
    [Fact]
    public void GivenIntersectingTimeRanges_Overlaps_ShouldReturnTrue()
    {
        var day = LocalDay("2022/01/10");
        TimeRange range1 = (day.At("10 am"), day.At("1:20 pm"));
        TimeRange range2 = (day.At("1 pm"), day.At("1:30 pm"));

        range1.Overlaps(range2).Should().BeTrue();
    }
    
    [Fact]
    public void GivenAdjacentTimeRanges_Overlaps_ShouldReturnFalse()
    {
        var day = LocalDay("2022/01/10");
        TimeRange range1 = (day.At("10 am"), day.At("1:20 pm"));
        TimeRange range2 = (day.At("1:20 pm"), day.At("1:50 pm"));

        range1.Overlaps(range2).Should().BeFalse();
    }
    
    [Fact]
    public void GivenIntersectingTimeRanges_GetOverlap_ShouldReturnTheOverlap()
    {
        var day = LocalDay("2022/01/10");
        TimeRange range1 = (day.At("10 am"), day.At("1:20 pm"));
        TimeRange range2 = (day.At("1 pm"), day.At("1:30 pm"));

        TimeRange overlap = (day.At("1 pm"), day.At("1:20 pm"));
        
        range1.GetOverlap(range2).Should().Be(overlap);
    }
    
    [Fact]
    public void GivenAPerfectOverlapTimeRanges_GetOverlap_ShouldReturnTheOverlap()
    {
        var day = LocalDay("2022/01/10");
        TimeRange range = (day.At("10 am"), day.At("1:20 pm"));
        
        range.GetOverlap(range).Should().Be(range);
    }
    
    [Fact]
    public void GivenNonOverlappingTimeRanges_GetOverlap_ShouldReturnNull()
    {
        var day = LocalDay("2022/01/10");
        TimeRange range1 = (day.At("10 am"), day.At("1:20 pm"));
        TimeRange range2 = (day.At("2 pm"), day.At("3:30 pm"));

        range1.GetOverlap(range2).Should().Be(default);
    }
    
    [Fact]
    public void GivenAdjacentTimeRanges_GetOverlap_ShouldReturnNull()
    {
        var day = LocalDay("2022/01/10");
        TimeRange range1 = (day.At("10 am"), day.At("1:20 pm"));
        TimeRange range2 = (day.At("1:20 pm"), day.At("3:30 pm"));

        range1.GetOverlap(range2).Should().Be(default);
    }
    
    [Fact]
    public void GivenATimeRangeAndANewStart_MoveTo_ShouldMoveTheTimeRangeToTheNewStartTime()
    {
        var day = LocalDay("2022/01/10");
        TimeRange range = (day.At("10 am"), day.At("10:20 am"));

        range.MoveTo(day.At("10:30 am")).Should().Be(new TimeRange(day.At("10:30 am"), day.At("10:50 am")));
    }
    
    [Fact]
    public void GivenATimeRangeAndANewStartResultingInAnInvalidDate_MoveTo_ShouldThrow()
    {
        var day = LocalDay("2022/01/10");
        TimeRange range = (day.At("10 am"), DateTimeOffset.MaxValue.AddMinutes(-10));
        Action invalidMove = () => range.MoveTo(day.At("11:00 am"));
        invalidMove.Should().Throw<ArgumentOutOfRangeException>();
    }
    
    [Fact]
    public void GivenATimeRangeToSplitIntoFourParts_SplitInto_ShouldReturnFourEquallySizeParts()
    {
        var day = LocalDay("2022/01/10");
        TimeRange range = (day.At("10 am"), day.At("12:00 pm"));

        var parts = (range / 4).ToArray();
        parts.SequenceEqual(range.SplitInto(4)).Should().BeTrue();
        parts.Should().HaveCount(4);
        parts.All(t => t.Duration == TimeSpan.FromMinutes(30)).Should().BeTrue();
    }
    
    [Fact]
    public void GivenATimeRangeToSplitIntoFiveParts_SplitInto_ShouldReturnFiveEquallySizeParts()
    {
        var day = LocalDay("2022/01/10");
        TimeRange range = (day.At("10 am"), day.At("12:00 pm"));

        var parts = (range / 5).ToArray();
        parts.SequenceEqual(range.SplitInto(5)).Should().BeTrue();
        parts.Should().HaveCount(5);
        parts.All(t => t.Duration == TimeSpan.FromMinutes(24)).Should().BeTrue();
    }
    
    [Fact]
    public void GivenATimeRangeToSplitIntoOnePart_SplitInto_ShouldReturnTheTimeRange()
    {
        var day = LocalDay("2022/01/10");
        TimeRange range = (day.At("10 am"), day.At("12:00 pm"));

        var parts = (range / 1).ToArray();
        parts.SequenceEqual(range.SplitInto(1)).Should().BeTrue();
        parts.Should().HaveCount(1);
        parts.All(t => t.Duration == TimeSpan.FromMinutes(120)).Should().BeTrue();
    }
    
    [Fact]
    public void GivenABlankTimeRangeToSplit_SplitInto_ShouldThrowArgumentException()
    {
        TimeRange range = default;

        Action splitBlank = () => range.SplitInto(4);
        splitBlank.Should().Throw<ArgumentException>();
    }
}