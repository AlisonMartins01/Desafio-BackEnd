using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Rentals.Application.Motorcycles
{
    public sealed record MotorcycleDto(
    Guid Id,
    string Identifier,
    int Year,
    string Model,
    string Plate
);
    
}
