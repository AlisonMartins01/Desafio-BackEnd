using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Rentals.Application.Abstractions;
using Rentals.Domain.Entities;
using Rentals.Domain.Enums;
using Rentals.Domain.Services;

namespace Rentals.Application.Rentals.Create
{
    public sealed class CreateRentalHandler : IRequestHandler<CreateRentalCommand, RentalDto>
    {
        private readonly ICourierRepository _couriers;
        private readonly IMotorcycleRepository _motos;
        private readonly IRentalRepository _rentals;
        private readonly IUnitOfWork _uow;
        private readonly IPricingService _pricing;

        public CreateRentalHandler(
            ICourierRepository couriers,
            IMotorcycleRepository motos,
            IRentalRepository rentals,
            IUnitOfWork uow,
            IPricingService pricing)
        {
            _couriers = couriers;
            _motos = motos;
            _rentals = rentals;
            _uow = uow;
            _pricing = pricing;
        }

        public async Task<RentalDto> Handle(CreateRentalCommand req, CancellationToken ct)
        {
            var courier = await _couriers.GetByIdentifierAsync(req.CourierIdentifier.Trim(), ct)
                          ?? throw new KeyNotFoundException("Entregador não encontrado.");

            if (!courier.CanRentMotorcycle())
                throw new InvalidOperationException("Entregador não habilitado na categoria A.");

            var moto = await _motos.GetByIdentifierAsync(req.MotorcycleIdentifier.Trim(), ct)
                       ?? throw new KeyNotFoundException("Motocicleta não encontrada.");

            if (await _rentals.HasActiveByMotorcycleAsync(moto.Id, ct))
                throw new InvalidOperationException("Motocicleta já está locada.");

            var plan = (RentalPlan)req.PlanDays;
            var (daily, _) = _pricing.For(plan);

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var rental = Rental.Create(courier.Id, moto.Id, plan, today, daily);

            await _rentals.AddAsync(rental, ct);
            await _uow.SaveChangesAsync(ct);

            return new RentalDto(
                rental.Id, courier.Id, courier.Identifier,
                moto.Id, moto.Identifier,
                (int)rental.Plan, rental.DailyRate,
                rental.StartDate, rental.ExpectedEndDate, rental.EndDate
            );
        }
    }
}
