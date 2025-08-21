using System.Text.Json.Serialization;

namespace Rentals.Api.Contracts
{
    public sealed record ErrorResponse(
     [property: JsonPropertyName("mensagem")] string Mensagem
 );
}
