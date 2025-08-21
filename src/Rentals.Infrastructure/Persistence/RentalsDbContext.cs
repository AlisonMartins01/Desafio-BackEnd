using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Rentals.Domain.Entities;

namespace Rentals.Infrastructure.Persistence
{
    public class RentalsDbContext : DbContext
    {
        public RentalsDbContext(DbContextOptions<RentalsDbContext> options) : base(options) { }

        public DbSet<Motorcycle> Motorcycles => Set<Motorcycle>();
        public DbSet<Courier> Couriers => Set<Courier>();
        public DbSet<Rental> Rentals => Set<Rental>();
        public DbSet<MotorcycleNotification> MotorcycleNotifications => Set<MotorcycleNotification>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(RentalsDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
