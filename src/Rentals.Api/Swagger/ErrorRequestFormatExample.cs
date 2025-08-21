using Rentals.Api.Contracts;
using Swashbuckle.AspNetCore.Filters;

namespace Rentals.Api.Swagger
{
    public sealed class ErrorRequestFormatExample : IExamplesProvider<ErrorResponse>
    {
        public ErrorResponse GetExamples() => new("Request mal formada");
    }
}
