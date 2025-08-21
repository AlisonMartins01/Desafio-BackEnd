using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Rentals.Application.Motorcycles.CreateMotorcycle
{
    public sealed class CreateMotorcycleCommandValidator : AbstractValidator<CreateMotorcycleCommand>
    {
        public CreateMotorcycleCommandValidator()
        {
            RuleFor(x => x.Identifier).NotEmpty().MaximumLength(80);
            RuleFor(x => x.Model).NotEmpty().MaximumLength(80);
            RuleFor(x => x.Year).InclusiveBetween(1900, DateTime.UtcNow.Year + 1);
            RuleFor(x => x.Plate).NotEmpty().MaximumLength(8);
        }
    }
}
