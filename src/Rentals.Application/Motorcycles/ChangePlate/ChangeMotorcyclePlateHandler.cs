using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Rentals.Application.Abstractions;
using Rentals.Domain.ValueObjects;

namespace Rentals.Application.Motorcycles.ChangePlate
{
    public sealed class ChangeMotorcyclePlateHandler : IRequestHandler<ChangeMotorcyclePlateCommand>
    {
        private readonly IMotorcycleRepository _repo;
        private readonly IUnitOfWork _uow;

        public ChangeMotorcyclePlateHandler(IMotorcycleRepository repo, IUnitOfWork uow)
        {
            _repo = repo;
            _uow = uow;
        }

        public async Task Handle(ChangeMotorcyclePlateCommand request, CancellationToken ct)
        {
            var moto = await _repo.GetByIdentifierAsync(request.Id, ct) ?? throw new KeyNotFoundException("Motocicleta não encontrada.");

            var normalized = request.NewPlate.Trim().ToUpperInvariant().Replace("-", "");
            if (await _repo.PlateExistsAsync(normalized, ct))
                throw new InvalidOperationException("Placa já cadastrada.");

            moto.ChangePlate(Plate.Create(normalized));

            await _uow.SaveChangesAsync(ct);
        }
    }
}
