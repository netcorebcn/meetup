using System;
using System.Collections.Generic;
using Xunit;

namespace Meetup.Domain.Tests
{
    public class DateTimeRangeTest
    {
        [Fact]
        public void Given_Valid_Range_When_Create_Then_Created()
        {
            var timeRange = DateTimeRange.From(date: "2019-06-19", time: "19:00", durationInHours: 3);
            Assert.Equal(new DateTime(2019, 6, 19, 19, 0, 0), timeRange.Start);
        }

        [Fact]
        public void Given_None_Range_When_Create_Then_Created()
        {
            var noneRange1 = DateTimeRange.None;
            var noneRange2 = DateTimeRange.None;

            Assert.Equal(DateTimeRange.None, noneRange1);
            Assert.Equal(noneRange2, noneRange1);
        }

        [Theory]
        [InlineData("2019/06/19", "19:00")]
        [InlineData("2019/06-19", "19:00")]
        [InlineData("19-06-19", "19:00")]
        [InlineData("2019-13-19", "19:00")]
        [InlineData("2019-12-32", "19:00")]
        [InlineData("2019-12-19", "24:01")]
        [InlineData("2019-12-19", "21:60")]
        public void Giving_wrong_date_or_pattern_should_thow_exception(string date, string time)
        {
            Assert.Throws<ArgumentException>(() =>
                DateTimeRange.From(date: date, time: time, durationInHours: 3)
            );
        }
    }
}
