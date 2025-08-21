using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentals.Application.Couriers
{
    public sealed record CourierDto(
    Guid Id,
    string Identifier,
    string Name,
    string Cnpj,
    DateOnly BirthDate,
    string CnhNumber,
    string LicenseType,
    string? CnhImageUrl
);
}
