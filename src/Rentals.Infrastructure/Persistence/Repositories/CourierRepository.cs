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
    public sealed class CourierRepository : ICourierRepository
    {
        private readonly RentalsDbContext _ctx;
        public CourierRepository(RentalsDbContext ctx) => _ctx = ctx;

        public Task AddAsync(Courier courier, CancellationToken ct) =>
            _ctx.Couriers.AddAsync(courier, ct).AsTask();

        public Task<Courier?> GetByIdAsync(Guid id, CancellationToken ct) =>
            _ctx.Couriers.FirstOrDefaultAsync(x => x.Id == id, ct);

        public Task<Courier?> GetByIdentifierAsync(string identifier, CancellationToken ct) =>
            _ctx.Couriers.FirstOrDefaultAsync(x => x.Identifier == identifier, ct);

        public Task<bool> CnpjExistsAsync(string cnpjDigits, CancellationToken ct)
        {
            var vo = Cnpj.Create(cnpjDigits);
            return _ctx.Couriers.AsNoTracking().AnyAsync(x => x.Cnpj == vo, ct);
        }

        public Task<bool> CnhNumberExistsAsync(string cnhDigits, CancellationToken ct)
        {
            var vo = CnhNumber.Create(cnhDigits);
            return _ctx.Couriers.AsNoTracking().AnyAsync(x => x.CnhNumber == vo, ct);
        }
    }
}
