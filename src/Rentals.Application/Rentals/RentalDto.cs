using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentals.Application.Rentals
{
    public sealed record RentalDto(
    Guid Id,
    Guid CourierId,
    string CourierIdentifier,
    Guid MotorcycleId,
    string MotorcycleIdentifier,
    int PlanDays,
    decimal DailyRate,
    DateOnly StartDate,
    DateOnly ExpectedEndDate,
    DateOnly? EndDate
);
}
