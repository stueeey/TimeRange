using System;
using System.Linq;
using FluentAssertions;
using Xunit;

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
            var _ = TimeRangeCollectionExtensions.ConsolidateSortedTimeRanges(ranges).ToArray();
        };
        attemptToConsolidateUnsortedList.Should().Throw<ArgumentException>();
    }
    
    [Fact]
    public void GivenASingleRangeAndManyExclusions_Exclude_ShouldReturnSlicesOfTime()
    {
        var day = LocalDay("2022/02/13");

        var range = new TimeRange(day.At("1 am"), day.At("9:30 pm"));

        var exclusions = new TimeRange[]
        {
            new(day, day.At("1:00 am")),
            
            new(day.At("2 am"), day.At("3:00 am")),
            new(day.At("3 am"), day.At("4:00 am")), // Neighbouring exclusion
            
            new(day.At("3:30 am"), day.At("9:00 am")), // Long overlapping exclusion
            new(day.At("5:00 am"), day.At("6:00 am")), // Within another exclusion
            
            new(day.At("9:00 pm"), day.At("10:00 pm")) // Overlapping end of time range
        };

        var result = range ^ exclusions;
        
        result.Should().BeEquivalentTo(new TimeRange[]
        {
            new(day.At("1 am"), day.At("2 am")),
            new(day.At("9 am"), day.At("9 pm"))
        });
    }
    
    [Fact]
    public void GivenManyRangesAndManyExclusions_Exclude_ShouldReturnSlicesOfTime()
    {
        var day = LocalDay("2022/02/13");

        var ranges = new TimeRange[]
        {
            (day.At("1 am"), day.At("9:30 pm"))   
        };

        var exclusions = new TimeRange[]
        {
            (day, day.At("1:00 am")),
            
            (day.At("2 am"), day.At("3:00 am")),
            (day.At("3 am"), day.At("4:00 am")), // Neighbouring exclusion
            
            (day.At("3:30 am"), day.At("9:00 am")), // Long overlapping exclusion
            (day.At("5:00 am"), day.At("6:00 am")), // Within another exclusion
            
            (day.At("9:00 pm"), day.At("10:00 pm")) // Overlapping end of time range
        };

        var result = ranges.Excluding(exclusions);
        
        result.Should().BeEquivalentTo(new TimeRange[]
        {
            new(day.At("1 am"), day.At("2 am")),
            new(day.At("9 am"), day.At("9 pm"))
        });
    }
    
}