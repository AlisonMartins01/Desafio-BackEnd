using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Rentals.Application.Motorcycles.ChangePlate
{
    public sealed class ChangeMotorcyclePlateValidator : AbstractValidator<ChangeMotorcyclePlateCommand>
    {
        public ChangeMotorcyclePlateValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.NewPlate).NotEmpty().MaximumLength(8);
        }
    }
}
