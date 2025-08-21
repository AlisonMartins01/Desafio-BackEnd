using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rentals.Domain.Entities;

namespace Rentals.Application.Abstractions
{
    public interface IMotorcycleRepository
    {
        Task AddAsync(Motorcycle motorcycle, CancellationToken ct);
        Task<Motorcycle?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<Motorcycle?> GetByIdentifierAsync(string identifier, CancellationToken ct);
        Task<Motorcycle?> GetByPlateAsync(string plate, CancellationToken ct);
        Task<bool> PlateExistsAsync(string plate, CancellationToken ct);
        Task RemoveAsync(Motorcycle motorcycle, CancellationToken ct);
    }
}
