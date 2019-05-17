using System;
using Xunit;

namespace Meetup.Domain.Tests
{
    public class SeatsNumberTest
    {
        [Fact]
        public void Given_Valid_NumberOfSeats_When_Create_Then_Created()
        {
            var number = SeatsNumber.From(10);
            Assert.Equal(10, number.Value);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Given_Invalid_NumberOfSeats_When_Create_Then_Exception(int invalidSeatsNumber) =>
            Assert.Throws<ArgumentException>(() => SeatsNumber.From(invalidSeatsNumber));
    }
}
