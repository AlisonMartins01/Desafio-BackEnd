using System.Text.Json.Serialization;

namespace Rentals.Api.Contracts.Motorcycle
{
    public sealed record MotorcycleResponse(
    [property: JsonPropertyName("identificador")] string Identifier,
    [property: JsonPropertyName("ano")] int Year,
    [property: JsonPropertyName("modelo")] string Model,
    [property: JsonPropertyName("placa")] string Plate
);
}
