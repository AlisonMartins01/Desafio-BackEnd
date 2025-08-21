using System.Text.Json.Serialization;
using Rentals.Api.Contracts;

namespace Rentals.Api.Swagger
{
    public sealed record BadRequestResponse(
    [property: JsonPropertyName("mensagem")] string Mensagem
);
}
