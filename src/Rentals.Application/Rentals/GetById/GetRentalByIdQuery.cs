using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Rentals.Application.Rentals.GetById
{
    public sealed record GetRentalByIdQuery(Guid Id) : IRequest<RentalDto>;
}
