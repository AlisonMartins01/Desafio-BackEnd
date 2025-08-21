using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rentals.Application.Abstractions;

namespace Rentals.Infrastructure.Persistence
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly RentalsDbContext _ctx;
        public UnitOfWork(RentalsDbContext ctx) => _ctx = ctx;

        public Task<int> SaveChangesAsync(CancellationToken ct)
            => _ctx.SaveChangesAsync(ct);
    }
}
