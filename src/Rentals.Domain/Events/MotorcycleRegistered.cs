using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rentals.Domain.Abstractions;

namespace Rentals.Domain.Events
{
    public sealed record MotorcycleRegistered(
    Guid MotorcycleId,
    string Identifier,
    int Year,
    string Model,
    string Plate
) : DomainEvent;
}
