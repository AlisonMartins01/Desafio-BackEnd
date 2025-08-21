using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Rentals.Application.Rentals.Create
{
    public sealed record CreateRentalCommand(
    string CourierIdentifier,
    string MotorcycleIdentifier,
    int PlanDays,              
    DateTime? RequestStartUtc,
    DateTime? RequestEndUtc,
    DateTime? RequestExpectedEndUtc
) : IRequest<RentalDto>;
}
