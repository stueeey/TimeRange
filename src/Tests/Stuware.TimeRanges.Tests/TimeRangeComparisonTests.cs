using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using static Stuware.TimeRanges.Tests.TestHelpers;

namespace Stuware.TimeRanges.Tests;

public class TimeRangeComparisonTests
{
    [Fact]
    public void GivenTwoIdenticalTimeRanges_Comparison_ShouldResultInEquality()
    {
        var day = LocalDay("2022/02/13");
        TimeRange range1 = (day.At("10 am"), day.At("11:34 pm"));
        TimeRange range2 = (day.At("10 am"), day.At("11:34 pm"));

        (range1 == range2).Should().BeTrue();
        (range1 >= range2).Should().BeTrue();
        (range1 <= range2).Should().BeTrue();
        (range1 != range2).Should().BeFalse();
        range1.Equals(range2).Should().BeTrue();
        range1.CompareTo(range2).Should().Be(0);
        range1.Should().Be(range2); // this will use the default comparer
        
    }
    
    [Fact]
    public void GivenTimeRangesWithDifferentStart_Comparison_ShouldNotResultInEquality()
    {
        var day = LocalDay("2022/02/13");
        TimeRange range1 = (day.At("10 am"), day.At("11:34 pm"));
        TimeRange range2 = (day.At("10:01 am"), day.At("11:34 pm"));

        (range1 == range2).Should().BeFalse();
        (range1 != range2).Should().BeTrue();
        range1.Equals(range2).Should().BeFalse();
        range1.CompareTo(range2).Should().NotBe(0);
        range1.Should().NotBe(range2); // this will use the default comparer
    }
    
    [Fact]
    public void GivenTimeRangesWithDifferentEnd_Comparison_ShouldNotResultInEquality()
    {
        var day = LocalDay("2022/02/13");
        TimeRange range1 = (day.At("10 am"), day.At("11:34 pm"));
        TimeRange range2 = (day.At("10 am"), day.At("11:35 pm"));

        (range1 == range2).Should().BeFalse();
        (range1 != range2).Should().BeTrue();
        range1.Equals(range2).Should().BeFalse();
        range1.CompareTo(range2).Should().NotBe(0);
        range1.Should().NotBe(range2); // this will use the default comparer
    }
    
    [Fact]
    public void GivenTimeRangesWithDifferentStart_Comparison_ShouldSortThemCorrectly()
    {
        var day = LocalDay("2022/02/13");
        TimeRange range1 = (day.At("10 am"), day.At("11:34 pm"));
        TimeRange range2 = (day.At("10:01 am"), day.At("11:34 pm"));
        TimeRange range3 = (day.At("10:02 am"), day.At("11:34 pm"));

        var unsortedList = new List<TimeRange> { range2, range1, range3 };
        unsortedList.Should().NotBeInAscendingOrder(Comparer<TimeRange>.Default);
        unsortedList.Sort();
        unsortedList.Should().BeInAscendingOrder(Comparer<TimeRange>.Default);
        unsortedList.SequenceEqual(new[]
        {
            range1,
            range2,
            range3
        }).Should().BeTrue();
        
        (range1 < range2).Should().BeTrue();
        (range1 > range2).Should().BeFalse();
        (range1 >= range2).Should().BeFalse();
        (range1 <= range2).Should().BeTrue();
    }
    
    [Fact]
    public void TimeRangesShouldBeUsableAsAKeyInADictionary()
    {
        var day = LocalDay("2022/02/13");
        TimeRange range1 = (day.At("10 am"), day.At("11:34 pm"));
        TimeRange range2 = (day.At("10:01 am"), day.At("11:34 pm"));
        TimeRange range3 = (day.At("10:02 am"), day.At("11:34 pm"));

        var dictionary = new Dictionary<TimeRange, string>
        {
            [range1] = nameof(range1),
            [range2] = nameof(range2),
            [range3] = nameof(range3)
        };

        // we want to make sure it isn't using referenceEquals to do the lookup 
        var range2Clone = new TimeRange(range2);

        dictionary[range2Clone].Should().Be(nameof(range2));
    }
    
    [Fact]
    public void GivenADateTimeOffset_Comparison_ShouldCorrectlySort()
    {
        var day = LocalDay("2022/02/13");
        TimeRange range1 = (day.At("10 am"), day.At("11:34 pm"));

        var beforeStart = day.At("9 am");
        (range1 > beforeStart).Should().BeTrue();
        (beforeStart < range1).Should().BeTrue();
        (range1 >= beforeStart).Should().BeTrue();
        (beforeStart <= range1).Should().BeTrue();
    }
}