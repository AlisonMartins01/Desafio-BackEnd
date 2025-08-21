using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Rentals.Application.Services;
using Rentals.Domain.Enums;
using Xunit;

namespace Rentals.UnitTests.Services
{
    public class PricingServiceTests
    {
        private readonly PricingService _svc = new();

        [Theory]
        [InlineData(RentalPlan.D7, 30, 0.20)]
        [InlineData(RentalPlan.D15, 28, 0.40)]
        [InlineData(RentalPlan.D30, 22, null)]
        [InlineData(RentalPlan.D45, 20, null)]
        [InlineData(RentalPlan.D50, 18, null)]
        public void For_plan_should_return_expected_daily_and_penalty(RentalPlan plan, decimal daily, double? penalty)
        {
            var (d, p) = _svc.For(plan);
            d.Should().Be(daily);

            if (penalty.HasValue)
                p.Should().BeApproximately((decimal)penalty.Value, 0.0001m);
            else
                p.Should().BeNull();
        }
    }
}
