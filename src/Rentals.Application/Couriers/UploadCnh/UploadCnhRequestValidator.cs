using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Rentals.Application.Couriers.UploadCnh
{
    public sealed class UploadCnhRequestValidator : AbstractValidator<UploadCnhImageCommand>
    {
        public UploadCnhRequestValidator()
        {
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
