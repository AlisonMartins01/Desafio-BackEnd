using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Rentals.Application.Couriers.Create
{
    public sealed class CreateCourierValidator : AbstractValidator<CreateCourierCommand>
    {
        public CreateCourierValidator()
        {
            RuleFor(x => x.Identifier).NotEmpty().MaximumLength(80);
            RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
            RuleFor(x => x.Cnpj).NotEmpty().MaximumLength(18); // com máscara aceita, normalizamos
            RuleFor(x => x.BirthDate).LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow));
            RuleFor(x => x.CnhNumber).NotEmpty().MaximumLength(14);
            RuleFor(x => x.LicenseType).NotEmpty().Must(v =>
                v.Equals("A", StringComparison.OrdinalIgnoreCase) ||
                v.Equals("B", StringComparison.OrdinalIgnoreCase) ||
                v.Equals("A+B", StringComparison.OrdinalIgnoreCase) ||
                v.Equals("AB", StringComparison.OrdinalIgnoreCase)
            ).WithMessage("Tipo de CNH deve ser A, B ou A+B.");

            RuleFor(x => x.ImagemCnh)
                .NotEmpty()
                .WithMessage("Imagem CNH é obrigatória")
                .Must(BeValidBase64)
                .WithMessage("Formato de imagem inválido");
        }
        private static bool BeValidBase64(string base64String)
        {
            if (string.IsNullOrWhiteSpace(base64String))
                return false;

            try
            {
                Convert.FromBase64String(base64String);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
