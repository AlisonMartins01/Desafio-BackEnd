using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MediatR;

namespace Rentals.Application.Couriers.UploadCnh
{
    public sealed record UploadCnhImageCommand(
            [property: JsonPropertyName("id")] string Identifier,
            [property: JsonPropertyName("imagem_cnh")] string ImagemCnh
) : IRequest<string>;
}
