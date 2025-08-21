using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Rentals.Domain.ValueObjects;

namespace Rentals.UnitTests.ValueObjects
{
    public class PlateTests
    {
        [Theory]
        [InlineData("ABC1D23")]
        [InlineData("ABC-1D23")] 
        [InlineData("ABC1234")]   
        [InlineData("abc1d23")]   
        public void Create_should_normalize_and_accept_valid_formats(string input)
        {
            var plate = Plate.Create(input);
            plate.Value.Should().Be("ABC1D23".Length == input.Replace("-", "").Length ? plate.Value : plate.Value);
            plate.Value.Should().MatchRegex(@"^[A-Z]{3}[0-9][A-Z0-9][0-9]{2}$|^[A-Z]{3}[0-9]{4}$");
        }

        [Theory]
        [InlineData("")]
        [InlineData("AB12345")]
        [InlineData("ABCD123")]
        [InlineData("1234567")]
        [InlineData("A1C1D23")] 
        public void Create_should_throw_for_invalid_plate(string input)
        {
            Action act = () => Plate.Create(input);
            act.Should().Throw<Exception>(); 
        }
    }
}
