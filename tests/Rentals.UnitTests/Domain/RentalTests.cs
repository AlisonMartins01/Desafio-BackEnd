using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Rentals.Application.Services;
using Rentals.Domain.Entities;
using Rentals.Domain.Enums;
using Rentals.Domain.Services;

namespace Rentals.UnitTests.Domain
{
    public class RentalTests
    {
        private readonly IPricingService _pricing = new PricingService();

        [Fact]
        public void Create_should_start_tomorrow_and_fill_expected_end()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var rental = Rental.Create(Guid.NewGuid(), Guid.NewGuid(), RentalPlan.D7, today, 30m);
            rental.StartDate.Should().Be(today.AddDays(1));
            rental.ExpectedEndDate.Should().Be(rental.StartDate.AddDays(7)); // Remove o -1
            rental.EndDate.Should().BeNull();
        }

        [Fact]
        public void CalculateReturnTotal_should_charge_penalty_for_early_return()
        {
            // Devolveu 2 dias antes => 6 diárias feitas + multa 20% sobre 1 diária restante
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var rental = Rental.Create(Guid.NewGuid(), Guid.NewGuid(), RentalPlan.D7, today, 30m);
            var returnDate = rental.ExpectedEndDate.AddDays(-2);
            var total = rental.CalculateReturnTotal(returnDate, _pricing);
            total.Should().Be(6 * 30m + 1 * 30m * 0.20m); // = 180 + 6 = 186
        }

        [Fact]
        public void CalculateReturnTotal_should_charge_extra_for_late_return()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var rental = Rental.Create(Guid.NewGuid(), Guid.NewGuid(), RentalPlan.D7, today, 30m);
            var returnDate = rental.ExpectedEndDate.AddDays(3);

            var total = rental.CalculateReturnTotal(returnDate, _pricing);

            // Correto: 10 dias × R$30 + 3 dias × R$50 = 300 + 150 = 450
            total.Should().Be(11 * 30m + 4 * 50m);
        }
    }
}
