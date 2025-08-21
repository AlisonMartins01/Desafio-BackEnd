using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Rentals.Infrastructure.Persistence
{
    public sealed class RentalsDbContextFactory : IDesignTimeDbContextFactory<RentalsDbContext>
    {
        public RentalsDbContext CreateDbContext(string[] args)
        {
            var cs = Environment.GetEnvironmentVariable("RENTALS_CS")
                     ?? "Host=localhost;Port=5432;Database=rentals;Username=postgres;Password=452313";

            var options = new DbContextOptionsBuilder<RentalsDbContext>()
                .UseNpgsql(cs)
                .Options;

            return new RentalsDbContext(options);
        }
    }
}
