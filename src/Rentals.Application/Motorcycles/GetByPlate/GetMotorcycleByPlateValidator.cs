using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Rentals.Application.Motorcycles.GetByPlate
{
    public sealed class GetMotorcycleByPlateValidator : AbstractValidator<GetMotorcycleByPlateQuery>
    {
        public GetMotorcycleByPlateValidator()
        {
            RuleFor(x => x.Plate).NotEmpty().MaximumLength(8);
        }
    }
}
