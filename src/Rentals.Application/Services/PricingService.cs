using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rentals.Domain.Enums;
using Rentals.Domain.Services;

namespace Rentals.Application.Services
{
    public sealed class PricingService : IPricingService
    {
        public (decimal Daily, decimal? EarlyPenaltyPct) For(RentalPlan plan) => plan switch
        {
            RentalPlan.D7 => (30m, 0.20m),
            RentalPlan.D15 => (28m, 0.40m),
            RentalPlan.D30 => (22m, null),
            RentalPlan.D45 => (20m, null),
            RentalPlan.D50 => (18m, null),
            _ => throw new ArgumentOutOfRangeException(nameof(plan), plan, null)
        };

        public decimal TotalFor(RentalPlan plan, decimal dailyRateSnapshot, int doneDays, int missingDays, int extraDays)
        {
            var (_, earlyPct) = For(plan);

            var baseCost = dailyRateSnapshot * doneDays;
            var early = earlyPct.HasValue ? earlyPct.Value * dailyRateSnapshot * missingDays : 0m;
            var late = 50m * extraDays;

            return Math.Round(baseCost + early + late, 2, MidpointRounding.AwayFromZero);
        }
    }
}
