using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rentals.Domain.Enums;

namespace Rentals.Domain.Services
{
    public interface IPricingService
    {
        (decimal Daily, decimal? EarlyPenaltyPct) For(RentalPlan plan);

        decimal TotalFor(RentalPlan plan, decimal dailyRateSnapshot, int doneDays, int missingDays, int extraDays);
    }
}
