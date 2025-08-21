using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rentals.Domain.Abstractions;
using Rentals.Domain.Enums;
using Rentals.Domain.Services;

namespace Rentals.Domain.Entities
{
    public sealed class Rental : Entity
    {
        public Guid CourierId { get; private set; }
        public Guid MotorcycleId { get; private set; }
        public RentalPlan Plan { get; private set; }
        public DateOnly StartDate { get; private set; }
        public DateOnly ExpectedEndDate { get; private set; }
        public DateOnly? EndDate { get; private set; }
        public decimal DailyRate { get; private set; } 
        public RentalStatus Status { get; private set; }

        private Rental() { } 

        private Rental(Guid courierId, Guid motorcycleId, RentalPlan plan, DateOnly startDate, decimal dailyRate)
        {
            DomainException.ThrowIf(courierId == Guid.Empty, "Courier inválido.");
            DomainException.ThrowIf(motorcycleId == Guid.Empty, "Moto inválida.");
            DomainException.ThrowIf(dailyRate <= 0, "Valor diário inválido.");

            CourierId = courierId;
            MotorcycleId = motorcycleId;
            Plan = plan;
            StartDate = startDate;
            ExpectedEndDate = startDate.AddDays((int)plan);
            DailyRate = dailyRate;
            Status = RentalStatus.Active;
        }

        public static Rental Create(Guid courierId, Guid motorcycleId, RentalPlan plan, DateOnly today, decimal dailyRate)
            => new(courierId, motorcycleId, plan, today.AddDays(1), dailyRate);

        public (int doneDays, int missingDays, int extraDays) DiffDays(DateOnly returnDate)
        {
            if (returnDate < StartDate) throw new DomainException("Devolução antes do início.");

            var done = (returnDate.ToDateTime(TimeOnly.MinValue) - StartDate.ToDateTime(TimeOnly.MinValue)).Days + 1;
            var planned = (int)Plan;

            int missing = 0;
            int extra = 0;

            if (done < planned) missing = planned - done;
            else if (done > planned) extra = done - planned;

            return (done, missing, extra);
        }

        public void Finish(DateOnly returnDate)
        {
            if (Status != RentalStatus.Active) throw new DomainException("Locação não está ativa.");
            _ = DiffDays(returnDate);
            EndDate = returnDate;
            Status = RentalStatus.Finished;
        }

        public decimal CalculateReturnTotal(DateOnly returnDate, IPricingService pricing)
        {
            var (done, missing, extra) = DiffDays(returnDate);
            return pricing.TotalFor(Plan, DailyRate, done, missing, extra);
        }
    }
}
