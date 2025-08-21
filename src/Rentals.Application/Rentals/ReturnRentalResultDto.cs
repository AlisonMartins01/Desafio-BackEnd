using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Rentals.Application.Rentals
{
    public sealed record ReturnRentalResultDto(
    Guid RentalId,
    decimal Total,
    int DoneDays,
    int MissingDays,
    int ExtraDays
);
}
