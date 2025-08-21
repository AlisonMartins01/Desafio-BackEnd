using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Rentals.Application.Abstractions;
using Rentals.Domain.Entities;

namespace Rentals.Infrastructure.Persistence.Repositories
{
    public sealed class RentalRepository : IRentalRepository
    {
        private readonly RentalsDbContext _ctx;
        public RentalRepository(RentalsDbContext ctx) => _ctx = ctx;

        public Task<bool> HasAnyByMotorcycleAsync(Guid motorcycleId, CancellationToken ct)
            => _ctx.Rentals.AsNoTracking().AnyAsync(r => r.MotorcycleId == motorcycleId, ct);
        public Task AddAsync(Rental rental, CancellationToken ct)
            => _ctx.Rentals.AddAsync(rental, ct).AsTask();

        public Task<Rental?> GetByIdAsync(Guid id, CancellationToken ct)
            => _ctx.Rentals.FirstOrDefaultAsync(r => r.Id == id, ct);

        public Task<bool> HasActiveByMotorcycleAsync(Guid motoId, CancellationToken ct)
            => _ctx.Rentals.AsNoTracking().AnyAsync(r => r.MotorcycleId == motoId && r.EndDate == null, ct);

        public Task<Rental?> GetByIdentifierAsync(Guid id, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
