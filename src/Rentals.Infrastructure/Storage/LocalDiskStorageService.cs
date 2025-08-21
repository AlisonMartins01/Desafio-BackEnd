using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Rentals.Domain.Services;

namespace Rentals.Infrastructure.Storage
{
    public sealed class LocalStorageOptions
    {
        public string RootPath { get; set; } = "storage";
        public string BaseUrl { get; set; } = "/storage";    
    }
    public sealed class LocalDiskStorageService : IStorageService
    {
        private readonly LocalStorageOptions _opt;
        private static readonly HashSet<string> Allowed = new(StringComparer.OrdinalIgnoreCase) { "image/png", "image/bmp" };

        public LocalDiskStorageService(IOptions<LocalStorageOptions> options) => _opt = options.Value;

        public async Task<string> SaveCnhImageAsync(string identifier, string fileName, string contentType, Stream content, CancellationToken ct)
        {
            if (!Allowed.Contains(contentType))
                throw new InvalidOperationException("Tipo de arquivo não permitido.");

            var ext = contentType.Equals("image/png", StringComparison.OrdinalIgnoreCase) ? ".png" : ".bmp";
            var dir = Path.Combine(_opt.RootPath, "cnh", identifier);
            Directory.CreateDirectory(dir);

            var filePath = Path.Combine(dir, "cnh" + ext);
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                await content.CopyToAsync(fs, ct);

            // Retorna caminho “acessável”. Se você servir estático, use BaseUrl; senão, retorne o FilePath mesmo.
            var publicPath = Path.Combine(_opt.BaseUrl.TrimEnd('/'), "cnh", identifier, "cnh" + ext)
                             .Replace('\\', '/');
            return publicPath;
        }
    }
}
