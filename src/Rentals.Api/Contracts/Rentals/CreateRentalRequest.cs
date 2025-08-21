using System.Text.Json.Serialization;

namespace Rentals.Api.Contracts.Rentals
{
    public sealed record CreateRentalRequest(
    [property: JsonPropertyName("entregador_id")] string EntregadorId,
    [property: JsonPropertyName("moto_id")] string MotoId,
    [property: JsonPropertyName("data_inicio")] DateTime? DataInicio,
    [property: JsonPropertyName("data_termino")] DateTime? DataTermino,
    [property: JsonPropertyName("data_previsao_termino")] DateTime? DataPrevisaoTermino,
    [property: JsonPropertyName("plano")] int Plano
);
}
