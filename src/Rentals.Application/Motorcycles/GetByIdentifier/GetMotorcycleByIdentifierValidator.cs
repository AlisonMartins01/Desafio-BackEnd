using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Rentals.Application.Motorcycles.GetByIdentifier
{
    public sealed class GetMotorcycleByIdentifierValidator : AbstractValidator<GetMotorcycleByIdentifierQuery>
    {
        public GetMotorcycleByIdentifierValidator()
        {
            RuleFor(x => x.Identifier)
            .NotEmpty()
            .WithMessage("Identificador é obrigatório")
            .Length(1, 50)
            .WithMessage("Identificador deve ter entre 1 e 50 caracteres");

        }
    }
}
