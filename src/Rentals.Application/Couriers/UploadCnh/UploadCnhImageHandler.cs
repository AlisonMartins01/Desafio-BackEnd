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

            // Valida se a string base64 não está vazia
            if (string.IsNullOrWhiteSpace(request.ImagemCnh))
                throw new DomainException("Imagem CNH é obrigatória.");

            // Decodifica a string base64
            byte[] imageBytes;
            try
            {
                imageBytes = Convert.FromBase64String(request.ImagemCnh);
            }
            catch (FormatException)
            {
                throw new DomainException("Formato de imagem inválido.");
            }

            // Valida o tipo de arquivo pela assinatura dos bytes (magic numbers)
            var fileType = GetImageType(imageBytes);
            //if (fileType != "png" && fileType != "bmp")
            //    throw new DomainException("Formato inválido. Envie png ou bmp.");

            // Cria o stream a partir dos bytes
            using var imageStream = new MemoryStream(imageBytes);

            // Gera nome único para o arquivo
            var fileName = $"cnh_{courier.Identifier}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.{fileType}";
            var contentType = $"image/{fileType}";

            // Salva no storage
            var url = await _storage.SaveCnhImageAsync(courier.Identifier, fileName, contentType, imageStream, ct);

            // Atualiza o courier com a URL da imagem
            courier.SetCnhImageUrl(url);

            await _uow.SaveChangesAsync(ct);
            return url;
        }
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
