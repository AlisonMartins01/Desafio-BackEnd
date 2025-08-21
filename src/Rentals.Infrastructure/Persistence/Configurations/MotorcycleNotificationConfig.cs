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
    public sealed class MotorcycleNotificationConfig : IEntityTypeConfiguration<MotorcycleNotification>
    {
        public void Configure(EntityTypeBuilder<MotorcycleNotification> b)
        {
            b.ToTable("motorcycle_notifications");
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.MotorcycleId).HasColumnName("motorcycle_id").IsRequired();
            b.Property(x => x.Identifier).HasColumnName("identifier").HasMaxLength(80).IsRequired();
            b.Property(x => x.Year).HasColumnName("year").IsRequired();
            b.Property(x => x.Reason).HasColumnName("reason").HasMaxLength(120).IsRequired();
            b.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();

            b.HasIndex(x => x.Identifier);
            b.HasIndex(x => x.Year);
        }
    }
}
