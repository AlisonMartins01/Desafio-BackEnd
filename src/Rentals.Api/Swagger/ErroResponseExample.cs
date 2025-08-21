using Rentals.Api.Contracts;
using Rentals.Application.Motorcycles;
using Swashbuckle.AspNetCore.Filters;

namespace Rentals.Api.Swagger
{
    public sealed class ErroResponseExample : IExamplesProvider<ErrorResponse>
    {
        public ErrorResponse GetExamples() => new("Dados inválidos");
    }
}
