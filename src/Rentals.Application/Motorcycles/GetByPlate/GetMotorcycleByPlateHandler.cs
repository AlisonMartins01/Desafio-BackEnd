using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Rentals.Application.Abstractions;

namespace Rentals.Application.Motorcycles.GetByPlate
{
    public sealed class GetMotorcycleByPlateHandler : IRequestHandler<GetMotorcycleByPlateQuery, MotorcycleDto>
    {
        private readonly IMotorcycleRepository _repo;
        public GetMotorcycleByPlateHandler(IMotorcycleRepository repo) => _repo = repo;

        public async Task<MotorcycleDto> Handle(GetMotorcycleByPlateQuery request, CancellationToken ct)
        {
            var normalized = request.Plate.Trim().ToUpperInvariant().Replace("-", "");
            var entity = await _repo.GetByPlateAsync(normalized, ct)
                        ?? throw new KeyNotFoundException("Motocicleta não encontrada.");

            return new MotorcycleDto(entity.Id, entity.Identifier, entity.Year, entity.Model, entity.Plate.Value);
        }
    }
}
