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
    int PlanDays,              // 7, 15, 30, 45, 50
    DateTime? RequestStartUtc, // vindos do body, só para validação/log
    DateTime? RequestEndUtc,
    DateTime? RequestExpectedEndUtc
) : IRequest<RentalDto>;
}
