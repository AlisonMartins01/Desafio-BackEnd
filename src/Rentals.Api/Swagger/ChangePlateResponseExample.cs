using Rentals.Api.Contracts;
using Swashbuckle.AspNetCore.Filters;

namespace Rentals.Api.Swagger
{
    public sealed class ChangePlateResponseExample : IExamplesProvider<ErrorResponse>
    {
        public ErrorResponse GetExamples() => new("Placa modificada com sucesso");
    }
}
