using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rentals.Domain.Entities;

namespace Rentals.Application.Abstractions
{
    public interface IRentalRepository
    {
        Task AddAsync(Rental rental, CancellationToken ct);
        Task<Rental?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<Rental?> GetByIdentifierAsync(Guid id, CancellationToken ct);
        Task<bool> HasActiveByMotorcycleAsync(Guid motorcycleId, CancellationToken ct);
        Task<bool> HasAnyByMotorcycleAsync(Guid motorcycleId, CancellationToken ct);
    }
}
