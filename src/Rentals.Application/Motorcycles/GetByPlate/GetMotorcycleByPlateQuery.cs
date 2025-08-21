using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Rentals.Application.Motorcycles.GetByPlate
{
    public sealed record GetMotorcycleByPlateQuery(string Plate) : IRequest<MotorcycleDto>;
}
