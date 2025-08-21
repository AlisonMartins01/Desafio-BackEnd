using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Rentals.Application.Abstractions;

namespace Rentals.Application.Rentals.GetById
{
    public sealed class GetRentalByIdHandler : IRequestHandler<GetRentalByIdQuery, RentalDto>
    {
        private readonly IRentalRepository _rentals;
        private readonly ICourierRepository _couriers;
        private readonly IMotorcycleRepository _motos;

        public GetRentalByIdHandler(IRentalRepository rentals, ICourierRepository couriers, IMotorcycleRepository motos)
        {
            _rentals = rentals;
            _couriers = couriers;
            _motos = motos;
        }

        public async Task<RentalDto> Handle(GetRentalByIdQuery request, CancellationToken ct)
        {
            var r = await _rentals.GetByIdAsync(request.Id, ct)
                    ?? throw new KeyNotFoundException("Locação não encontrada.");

            var courier = await _couriers.GetByIdAsync(r.CourierId, ct)
                          ?? throw new KeyNotFoundException("Entregador não encontrado.");
            var moto = await _motos.GetByIdAsync(r.MotorcycleId, ct)
                       ?? throw new KeyNotFoundException("Motocicleta não encontrada.");

            return new RentalDto(
                r.Id, courier.Id, courier.Identifier,
                moto.Id, moto.Identifier,
                (int)r.Plan, r.DailyRate, r.StartDate, r.ExpectedEndDate, r.EndDate
            );
        }
    }
}
