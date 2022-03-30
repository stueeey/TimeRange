using System.Linq;
using FluentAssertions;
using Xunit;
using static Stuware.TimeRanges.Tests.TestHelpers;

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

        range1.GetOverlap(range2).Should().BeNull();
    }
    
    [Fact]
    public void GivenAdjacentTimeRanges_GetOverlap_ShouldReturnNull()
    {
        var day = LocalDay("2022/01/10");
        TimeRange range1 = (day.At("10 am"), day.At("1:20 pm"));
        TimeRange range2 = (day.At("1:20 pm"), day.At("3:30 pm"));

        range1.GetOverlap(range2).Should().BeNull();
    }
}