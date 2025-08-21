using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Rentals.Application.Abstractions;
using Rentals.Application.Exceptions;

namespace Rentals.Application.Motorcycles.GetByIdentifier
{
    public sealed class GetMotorcycleByIdentifierHandler
    : IRequestHandler<GetMotorcycleByIdentifierQuery, MotorcycleDto>
    {
        private readonly IMotorcycleRepository _repo;
        public GetMotorcycleByIdentifierHandler(IMotorcycleRepository repo) => _repo = repo;

        public async Task<MotorcycleDto> Handle(GetMotorcycleByIdentifierQuery request, CancellationToken ct)
        {
            var idf = request.Identifier.Trim();
            var entity = await _repo.GetByIdentifierAsync(idf, ct)
                         ?? throw new MotorcycleNotFoundException();

            return new MotorcycleDto(entity.Id, entity.Identifier, entity.Year, entity.Model, entity.Plate.Value);
        }
    }
}
