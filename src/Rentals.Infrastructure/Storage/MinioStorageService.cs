using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Rentals.Domain.Services;

namespace Rentals.Infrastructure.Storage
{
    public sealed class MinioOptions
    {
        public string Endpoint { get; set; } = default!;
        public string AccessKey { get; set; } = default!;
        public string SecretKey { get; set; } = default!;
        public string Bucket { get; set; } = "cnh-images";
        public string? PublicBaseUrl { get; set; }            
    }

    public sealed class MinioStorageService : IStorageService
    {
        private readonly MinioOptions _opt;
        private readonly IMinioClient _client;

        public MinioStorageService(IOptions<MinioOptions> options)
        {
            _opt = options.Value;

            var uri = new Uri(_opt.Endpoint); // suporta "http://host:port" ou "https://..."
            var builder = new MinioClient()
                .WithEndpoint(uri.Host, uri.Port)
                .WithCredentials(_opt.AccessKey, _opt.SecretKey);

            if (uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
                builder = builder.WithSSL();

            _client = builder.Build();
        }

        public async Task<string> SaveCnhImageAsync(string identifier, string fileName, string contentType, Stream content, CancellationToken ct)
        {
            // Garante bucket
            bool exists = await _client.BucketExistsAsync(new BucketExistsArgs().WithBucket(_opt.Bucket), ct);
            if (!exists)
                await _client.MakeBucketAsync(new MakeBucketArgs().WithBucket(_opt.Bucket), ct);

            var ext = contentType.Equals("image/png", StringComparison.OrdinalIgnoreCase) ? ".png" : ".bmp";
            var objectName = $"{identifier:N}/cnh{ext}";

            // PutObject: se o stream não tiver Length, bufferizamos (arquivo é pequeno)
            Stream uploadStream = content;
            long size;

            if (content.CanSeek)
            {
                size = content.Length - content.Position;
            }
            else
            {
                var ms = new MemoryStream();
                await content.CopyToAsync(ms, ct);
                ms.Position = 0;
                uploadStream = ms;
                size = ms.Length;
            }

            var put = new PutObjectArgs()
                .WithBucket(_opt.Bucket)
                .WithObject(objectName)
                .WithStreamData(uploadStream)
                .WithObjectSize(size)
                .WithContentType(contentType);

            await _client.PutObjectAsync(put, ct);

            // URL pública (se configurada) ou pré-assinada (privado)
            if (!string.IsNullOrWhiteSpace(_opt.PublicBaseUrl))
                return $"{_opt.PublicBaseUrl!.TrimEnd('/')}/{objectName}";

            var presigned = await _client.PresignedGetObjectAsync(
                new PresignedGetObjectArgs()
                    .WithBucket(_opt.Bucket)
                    .WithObject(objectName)
                    .WithExpiry(60 * 60) // 1h
            );

            return presigned;
        }
    }
}
