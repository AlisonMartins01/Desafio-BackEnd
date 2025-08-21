using System.Text.Json.Serialization;

namespace Rentals.Api.Contracts.Couriers
{
    public sealed record CreateCourierRequest(
    [property: JsonPropertyName("identificador")] string Identificador,
    [property: JsonPropertyName("nome")] string Nome,
    [property: JsonPropertyName("cnpj")] string Cnpj,
    [property: JsonPropertyName("data_nascimento")] DateOnly DataNascimento,
    [property: JsonPropertyName("numero_cnh")] string NumeroCnh,
    [property: JsonPropertyName("tipo_cnh")] string TipoCnh,
    [property: JsonPropertyName("imagem_cnh")] string ImagemCnh
);
}
