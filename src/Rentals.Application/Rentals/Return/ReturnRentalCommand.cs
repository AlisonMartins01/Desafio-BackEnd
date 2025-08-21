using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Rentals.Application.Rentals.Return
{
    public sealed record ReturnRentalCommand(Guid RentalId, DateOnly ReturnDate) : IRequest<ReturnRentalResultDto>;

}
