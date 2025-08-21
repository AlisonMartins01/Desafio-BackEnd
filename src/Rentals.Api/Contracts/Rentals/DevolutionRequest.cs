using System.Text.Json.Serialization;

namespace Rentals.Api.Contracts.Rentals
{
    public sealed record DevolutionRequest(
    [property: JsonPropertyName("data_devolucao")] DateTime DataDevolucao
);
}
