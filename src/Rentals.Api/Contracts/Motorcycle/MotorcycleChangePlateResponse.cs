using System.Text.Json.Serialization;

namespace Rentals.Api.Contracts.Motorcycle{
    public sealed record MotorcMotorcycleChangePlateResponseycleResponse(
        [property: JsonPropertyName("placa")] string Plate
    );

}
