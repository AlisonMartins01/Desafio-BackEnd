using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Rentals.Application.Abstractions;
using Rentals.Domain.Abstractions;
using Rentals.Domain.Entities;
using Rentals.Domain.Enums;
using Rentals.Domain.Services;
using Rentals.Domain.ValueObjects;

namespace Rentals.Application.Couriers.Create
{
    public sealed class CreateCourierHandler : IRequestHandler<CreateCourierCommand, CourierDto>
    {
        private readonly ICourierRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly IStorageService _storage;

        public CreateCourierHandler(ICourierRepository repo, IUnitOfWork uow, IStorageService storage)
        {
            _repo = repo;
            _uow = uow;
            _storage = storage;
        }

        public async Task<CourierDto> Handle(CreateCourierCommand request, CancellationToken ct)
        {
            var cnpjDigits = new string(request.Cnpj.Where(char.IsDigit).ToArray());
            var cnhDigits = new string(request.CnhNumber.Where(char.IsDigit).ToArray());

            if (await _repo.CnpjExistsAsync(cnpjDigits, ct))
                throw new InvalidOperationException("CNPJ já cadastrado.");
            if (await _repo.CnhNumberExistsAsync(cnhDigits, ct))
                throw new InvalidOperationException("CNH já cadastrada.");

            byte[] imageBytes;
            try
            {
                imageBytes = Convert.FromBase64String(request.ImagemCnh);
            }
            catch (FormatException)
            {
                throw new DomainException("Formato de imagem inválido.");
            }

            var fileType = GetImageType(imageBytes);
            if (fileType != "png" && fileType != "bmp")
                throw new DomainException("Formato inválido. Envie png ou bmp.");

            // Cria o stream a partir dos bytes
            using var imageStream = new MemoryStream(imageBytes);

            // Gera nome único para o arquivo
            var fileName = $"cnh_{request.Identifier}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.{fileType}";
            var contentType = $"image/{fileType}";

            // Salva no storage
            var url = await _storage.SaveCnhImageAsync(request.Identifier, fileName, contentType, imageStream, ct);

            var license = ParseLicense(request.LicenseType);

            var entity = Courier.Create(
                request.Identifier,
                request.Name,
                Cnpj.Create(cnpjDigits),
                request.BirthDate,
                CnhNumber.Create(cnhDigits),
                license
            );

            entity.SetCnhImageUrl(url);

            await _repo.AddAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);

            return new CourierDto(
                entity.Id, entity.Identifier, entity.Name, entity.Cnpj.Digits, entity.BirthDate,
                entity.CnhNumber.Digits, entity.LicenseType.ToString(), entity.CnhImageUrl
            );
        }

        private static LicenseType ParseLicense(string s) =>
            s.ToUpperInvariant() switch
            {
                "A" => LicenseType.A,
                "B" => LicenseType.B,
                "A+B" or "AB" => LicenseType.AB,
                _ => throw new ArgumentOutOfRangeException(nameof(s), "Tipo de CNH inválido.")
            };

        private static string GetImageType(byte[] imageBytes)
        {
            if (imageBytes.Length < 8) return "unknown";

            // PNG: 89 50 4E 47 0D 0A 1A 0A
            if (imageBytes[0] == 0x89 && imageBytes[1] == 0x50 && imageBytes[2] == 0x4E && imageBytes[3] == 0x47)
                return "png";

            // BMP: 42 4D
            if (imageBytes[0] == 0x42 && imageBytes[1] == 0x4D)
                return "bmp";

            return "unknown";
        }
    }
}
