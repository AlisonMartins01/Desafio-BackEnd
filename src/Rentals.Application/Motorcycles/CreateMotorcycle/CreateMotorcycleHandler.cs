using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Rentals.Application.Abstractions;
using Rentals.Application.Contracts.Events;
using Rentals.Domain.Entities;
using Rentals.Domain.ValueObjects;

namespace Rentals.Application.Motorcycles.CreateMotorcycle
{
    public sealed class CreateMotorcycleHandler : IRequestHandler<CreateMotorcycleCommand, MotorcycleDto>
    {
        private readonly IMotorcycleRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly IPublishEndpoint _publish;

        public CreateMotorcycleHandler(IMotorcycleRepository repo, IUnitOfWork uow, IPublishEndpoint publishEndpoint)
        {
            _repo = repo;
            _uow = uow;
            _publish = publishEndpoint;
        }

        public async Task<MotorcycleDto> Handle(CreateMotorcycleCommand request, CancellationToken ct)
        {
            var normalizedPlate = request.Plate.Trim().ToUpperInvariant().Replace("-", "");
            if (await _repo.PlateExistsAsync(normalizedPlate, ct))
                throw new InvalidOperationException("Placa já cadastrada.");

            var entity = Motorcycle.Create(
                request.Identifier.Trim(),
                request.Year,
                request.Model.Trim(),
                Plate.Create(normalizedPlate)
            );

            await _repo.AddAsync(entity, ct);

            await _publish.Publish<IMotorcycleRegistered>(new
            {
                Id = entity.Id,
                Identifier = entity.Identifier,
                Model = entity.Model,
                Year = entity.Year,
                Plate = entity.Plate.Value,
                CreatedAtUtc = DateTime.UtcNow
            }, ct);

            await _uow.SaveChangesAsync(ct);

            return new MotorcycleDto(entity.Id, entity.Identifier, entity.Year, entity.Model, entity.Plate.Value);
        }
    }
}
