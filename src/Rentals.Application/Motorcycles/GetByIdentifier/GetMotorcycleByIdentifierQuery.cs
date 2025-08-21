using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Rentals.Application.Motorcycles.GetByIdentifier
{
    public sealed record GetMotorcycleByIdentifierQuery(string Identifier) : IRequest<MotorcycleDto>;
}
