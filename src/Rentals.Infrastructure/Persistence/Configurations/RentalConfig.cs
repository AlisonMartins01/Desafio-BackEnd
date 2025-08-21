using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rentals.Domain.Entities;

namespace Rentals.Infrastructure.Persistence.Configurations
{
    public class RentalConfig : IEntityTypeConfiguration<Rental>
    {
        public void Configure(EntityTypeBuilder<Rental> b)
        {
            b.ToTable("rentals");
            b.HasKey(x => x.Id);

            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.CourierId).HasColumnName("courier_id").IsRequired();
            b.Property(x => x.MotorcycleId).HasColumnName("motorcycle_id").IsRequired();

            b.Property(x => x.Plan).HasColumnName("plan").HasConversion<int>().IsRequired();
            b.Property(x => x.StartDate).HasColumnName("start_date").HasColumnType("date").IsRequired();
            b.Property(x => x.ExpectedEndDate).HasColumnName("expected_end_date").HasColumnType("date").IsRequired();
            b.Property(x => x.EndDate).HasColumnName("end_date").HasColumnType("date");
            b.Property(x => x.DailyRate).HasColumnName("daily_rate").HasColumnType("numeric(10,2)").IsRequired();
            b.Property(x => x.Status).HasColumnName("status").HasConversion<int>().IsRequired();

            b.HasOne<Courier>().WithMany().HasForeignKey(x => x.CourierId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne<Motorcycle>().WithMany().HasForeignKey(x => x.MotorcycleId).OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(x => new { x.CourierId, x.Status });
            b.HasIndex(x => new { x.MotorcycleId, x.Status });
        }
    }
}
