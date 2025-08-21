using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rentals.Domain.Entities;
using Rentals.Infrastructure.Persistence.Converters;

namespace Rentals.Infrastructure.Persistence.Configurations
{
    public class MotorcycleConfig : IEntityTypeConfiguration<Motorcycle>
    {
        public void Configure(EntityTypeBuilder<Motorcycle> b)
        {
            b.ToTable("motorcycles");
            b.HasKey(x => x.Id);

            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.Identifier).HasColumnName("identifier").HasMaxLength(80).IsRequired();
            b.Property(x => x.Model).HasColumnName("model").HasMaxLength(80).IsRequired();
            b.Property(x => x.Year).HasColumnName("year").IsRequired();

            b.Property(x => x.Plate)
                .HasConversion(new PlateConverter())
                .HasColumnName("plate")
                .HasMaxLength(8)
                .IsRequired();

            b.HasIndex(x => x.Identifier).IsUnique();
            b.HasIndex(x => x.Plate).IsUnique();
        }
    }
}
