using System;
using System.Linq;
using FluentAssertions;
using Xunit;
using static Stuware.TimeRanges.Tests.TestHelpers;

namespace Stuware.TimeRanges.Tests;

public class TimeRangeCollectionExtensionTests
{
    [Fact]
    public void GivenNonOverlappingRanges_Consolidate_ShouldReturnThemAsIs()
    {
        var day = LocalDay("2022/02/13");

        var ranges = new TimeRange[]
        {
            new (day.At("10 am"), day.At("12 pm")),
            new (day.At("1 pm"), day.At("2 pm")),
        };

        ranges.Consolidate().Should().BeEquivalentTo(ranges);
    }
    
    [Fact]
    public void GivenPerfectlyOverlappingRanges_Consolidate_ShouldReturnASingleRange()
    {
        var day = LocalDay("2022/02/13");

        var ranges = new TimeRange[]
        {
            new (day.At("10 am"), day.At("12 pm")),
            new (day.At("10 am"), day.At("12 pm")),
        };

        ranges.Consolidate().Should().BeEquivalentTo(new TimeRange[]
        {
            new (day.At("10 am"), day.At("12 pm"))
        });
    }
    
    [Fact]
    public void GivenOverlappingRanges_Consolidate_ShouldReturnTheAggregateOfThem()
    {
        var day = LocalDay("2022/02/13");

        var ranges = new TimeRange[]
        {
            // Simple intersection
            new (day.At("10 am"), day.At("12 pm")),
            new (day.At("11 am"), day.At("2 pm")),
            
            // Multiple overlaps
            new (day.At("1 am"), day.At("2 am")),
            new (day.At("2 am"), day.At("9 am")),
            new (day.At("2 am"), day.At("3 am")),
            new (day.At("4 am"), day.At("9:30 am")),
        };

        ranges.Consolidate().Should().BeEquivalentTo(new TimeRange[]
        {
            new (day.At("1 am"), day.At("9:30 am")),
            new (day.At("10 am"), day.At("2 pm"))
        });
    }
    
    [Fact]
    public void GivenNoRanges_Consolidate_ShouldReturnAnEmptyEnumerable()
    {
        Array.Empty<TimeRange>().Consolidate().Should().BeEmpty();
    }
    
    [Fact]
    public void GivenASingleRange_Consolidate_ShouldReturnASingleRange()
    {
        var day = LocalDay("2022/02/13");

        var range = new TimeRange[]
        {
            new(day.At("1 am"), day.At("9:30 am")),
        };
        
        range.Consolidate().Should().BeEquivalentTo(range);
    }
    
    [Fact]
    public void GivenAUnsortedRanges_ConsolidateSortedTimeRanges_ShouldThrowArgumentException()
    {
        var day = LocalDay("2022/02/13");

        var ranges = new TimeRange[]
        {
            // Simple intersection
            new (day.At("10 am"), day.At("12 pm")),
            new (day.At("11 am"), day.At("2 pm")),
            
            // Multiple overlaps
            new (day.At("1 am"), day.At("2 am")),
            new (day.At("2 am"), day.At("9 am")),
            new (day.At("2 am"), day.At("3 am")),
            new (day.At("4 am"), day.At("9:30 am")),
        };

        var attemptToConsolidateUnsortedList = () =>
        {
            var _ = ranges.ConsolidateSortedTimeRanges().ToArray();
        };
        attemptToConsolidateUnsortedList.Should().Throw<ArgumentException>();
    }
}