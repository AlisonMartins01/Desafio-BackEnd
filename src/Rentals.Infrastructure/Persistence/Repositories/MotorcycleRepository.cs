using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Rentals.Application.Abstractions;
using Rentals.Domain.Entities;
using Rentals.Domain.ValueObjects;

namespace Rentals.Infrastructure.Persistence.Repositories
{
    public sealed class MotorcycleRepository : IMotorcycleRepository
    {
        private readonly RentalsDbContext _ctx;
        public MotorcycleRepository(RentalsDbContext ctx) => _ctx = ctx;

        public async Task AddAsync(Motorcycle motorcycle, CancellationToken ct)
            => await _ctx.Motorcycles.AddAsync(motorcycle, ct);

        public Task<Motorcycle?> GetByIdAsync(Guid id, CancellationToken ct)
            => _ctx.Motorcycles.FirstOrDefaultAsync(m => m.Id == id, ct);

        public Task<Motorcycle?> GetByIdentifierAsync(string identifier, CancellationToken ct)
            => _ctx.Motorcycles.FirstOrDefaultAsync(m => m.Identifier == identifier, ct);
        public Task<Motorcycle?> GetByPlateAsync(string plate, CancellationToken ct)
        {
            var vo = Plate.Create(plate.Trim().ToUpperInvariant().Replace("-", ""));
            return _ctx.Motorcycles.FirstOrDefaultAsync(m => m.Plate == vo, ct);
        }
        public Task<bool> PlateExistsAsync(string plate, CancellationToken ct)
        {
            var vo = Plate.Create(plate.Trim().ToUpperInvariant().Replace("-", ""));
            return _ctx.Motorcycles.AsNoTracking().AnyAsync(m => m.Plate == vo, ct);
        }

        public Task RemoveAsync(Motorcycle motorcycle, CancellationToken ct)
        {
            _ctx.Motorcycles.Remove(motorcycle);
            return Task.CompletedTask;
        }
    }
}
