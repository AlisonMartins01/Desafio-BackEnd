using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Rentals.Domain.ValueObjects;

namespace Rentals.Infrastructure.Persistence.Converters
{
    public sealed class CnhNumberConverter : ValueConverter<CnhNumber, string>
    {
        public CnhNumberConverter() : base(
            v => v.Digits,
            v => CnhNumber.Create(v))
        { }
    }
}
