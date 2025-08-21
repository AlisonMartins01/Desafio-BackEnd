using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rentals.Domain.Entities;
using Rentals.Infrastructure.Persistence.Converters;

namespace Rentals.Infrastructure.Persistence.Configurations
{
    public class CourierConfig : IEntityTypeConfiguration<Courier>
    {
        public void Configure(EntityTypeBuilder<Courier> b)
        {
            b.ToTable("couriers");
            b.HasKey(x => x.Id);

            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.Identifier).HasColumnName("identifier").HasMaxLength(80).IsRequired(); // novo
            b.Property(x => x.Name).HasColumnName("name").HasMaxLength(120).IsRequired();
            b.Property(x => x.BirthDate).HasColumnName("birth_date").HasColumnType("date").IsRequired();
            b.Property(x => x.LicenseType).HasColumnName("license_type").HasConversion<int>().IsRequired();

            b.Property(x => x.Cnpj)
                .HasConversion(new CnpjConverter())
                .HasColumnName("cnpj")
                .HasMaxLength(14)
                .IsRequired();

            b.Property(x => x.CnhNumber)
                .HasConversion(new CnhNumberConverter())
                .HasColumnName("cnh_number")
                .HasMaxLength(14)
                .IsRequired();

            b.Property(x => x.CnhImageUrl).HasColumnName("cnh_image_url").HasMaxLength(512);

            b.HasIndex(x => x.Cnpj).IsUnique();
            b.HasIndex(x => x.CnhNumber).IsUnique();
            b.HasCheckConstraint("ck_courier_license_type", "license_type in (1,2,3)");
        }
    }
}
