using Rentals.Api.Contracts;
using Swashbuckle.AspNetCore.Filters;

namespace Rentals.Api.Swagger
{
    public sealed class ErrorNotFoundRentalExample : IExamplesProvider<ErrorResponse>
    {
        public ErrorResponse GetExamples() => new("Locação não encontrada");
    }
}
