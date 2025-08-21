using Rentals.Api.Contracts;
using Swashbuckle.AspNetCore.Filters;

namespace Rentals.Api.Swagger
{
    public sealed class ErrorNotFoundMotorcycleExample : IExamplesProvider<ErrorResponse>
    {
        public ErrorResponse GetExamples() => new("Moto não encontrada");
    }
}
