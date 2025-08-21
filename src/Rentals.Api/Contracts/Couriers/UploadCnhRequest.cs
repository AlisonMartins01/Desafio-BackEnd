using System.Text.Json.Serialization;

namespace Rentals.Api.Contracts.Couriers
{ 
    public sealed record UploadCnhRequest(
        [property: JsonPropertyName("imagem_cnh")] string ImagemCnh
    );
}
