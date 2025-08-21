using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Rentals.Api.Contracts.Motorcycle
{
    public sealed class ChangePlateRequest
    {
        [JsonPropertyName("placa")]
        public required string NewPlate { get; set; }
    }
}
