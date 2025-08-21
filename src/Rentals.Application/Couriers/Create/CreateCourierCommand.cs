using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Rentals.Application.Couriers.Create
{
    public sealed record CreateCourierCommand(
    string Identifier,
    string Name,
    string Cnpj,
    DateOnly BirthDate,
    string CnhNumber,
    string LicenseType, // "A", "B", "A+B" (ou "AB")
    string ImagemCnh
) : IRequest<CourierDto>;
}
