using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Rentals.Application.Abstractions;
using Rentals.Domain.Abstractions;
using Rentals.Domain.Services;

namespace Rentals.Application.Couriers.UploadCnh
{
    public sealed class UploadCnhImageHandler : IRequestHandler<UploadCnhImageCommand, string>
    {
        private readonly ICourierRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly IStorageService _storage;

        public UploadCnhImageHandler(ICourierRepository repo, IUnitOfWork uow, IStorageService storage)
        {
            _repo = repo;
            _uow = uow;
            _storage = storage;
        }

        public async Task<string> Handle(UploadCnhImageCommand request, CancellationToken ct)
        {
            var courier = await _repo.GetByIdentifierAsync(request.Identifier, ct)
                          ?? throw new KeyNotFoundException("Entregador não encontrado.");

            if (string.IsNullOrWhiteSpace(request.ImagemCnh))
                throw new DomainException("Imagem CNH é obrigatória.");

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

            using var imageStream = new MemoryStream(imageBytes);

            var fileName = $"cnh_{courier.Identifier}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.{fileType}";
            var contentType = $"image/{fileType}";

            var url = await _storage.SaveCnhImageAsync(courier.Identifier, fileName, contentType, imageStream, ct);

            courier.SetCnhImageUrl(url);

            await _uow.SaveChangesAsync(ct);
            return url;
        }
        private static string GetImageType(byte[] imageBytes)
        {
            if (imageBytes.Length < 8) return "unknown";

            if (imageBytes[0] == 0x89 && imageBytes[1] == 0x50 && imageBytes[2] == 0x4E && imageBytes[3] == 0x47)
                return "png";

            if (imageBytes[0] == 0x42 && imageBytes[1] == 0x4D)
                return "bmp";

            return "unknown";
        }
    }
}
