using System.Text.Json.Serialization;

namespace Rentals.Api.Contracts.Rentals
{
    public sealed record DevolutionResponse(
    [property: JsonPropertyName("valor_total")] decimal ValorTotal,
    [property: JsonPropertyName("dias_realizados")] int DiasRealizados,
    [property: JsonPropertyName("dias_faltantes")] int DiasFaltantes,
    [property: JsonPropertyName("dias_extras")] int DiasExtras
);
}
