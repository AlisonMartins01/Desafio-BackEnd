using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Rentals.Application.Motorcycles.Delete
{
    public sealed record DeleteMotorcycleCommand(string Id) : IRequest;
}
