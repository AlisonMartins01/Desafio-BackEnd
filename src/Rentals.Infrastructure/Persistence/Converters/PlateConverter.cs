using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rentals.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;


namespace Rentals.Infrastructure.Persistence.Converters
{
    public sealed class PlateConverter : ValueConverter<Plate, string>
    {
        public PlateConverter() : base(
            v => v.Value,
            v => Plate.Create(v))
        { }
    }
}
