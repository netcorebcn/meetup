using System;
using System.Collections.Generic;
using Xunit;

namespace Meetup.Domain.Tests
{
    public class AddressTest
    {
        [Fact]
        public void Given_Same_Address_When_Compare_Then_Equal()
        {
            var plainAddress = "Seattle, Redmond";
            var address1 = Address.From(plainAddress);
            var address2 = Address.From(plainAddress);
            Assert.Equal(address1, address2);
            Assert.True(address1 == address2);
        }
    }
}
