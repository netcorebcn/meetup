using System;
using System.Collections.Generic;
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

        [Fact]
        public void Given_Same_NumberOfSeats_When_Compare_Then_Equal()
        {
            var number1 = SeatsNumber.From(10);
            var number2 = SeatsNumber.From(10);

            Assert.Equal(number1, number2);
            Assert.True(number1 == number2);
            Assert.False(number1 != number2);

            var numbers = new Dictionary<SeatsNumber, string>();
            numbers.Add(number1, "meetup1");
            Assert.Throws<ArgumentException>(() => numbers.Add(number2, "meetup2"));
        }

        [Fact]
        public void Given_Same_NumberOfSeats_When_Use_AsDictionaryKey_Then_Exception()
        {
            var number1 = SeatsNumber.From(10);
            var number2 = SeatsNumber.From(10);
            var numbers = new Dictionary<SeatsNumber, string>();

            numbers.Add(number1, "meetup1");
            Assert.Throws<ArgumentException>(() => numbers.Add(number2, "meetup2"));
        }
    }
}
