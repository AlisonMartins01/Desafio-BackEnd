using Rentals.Api.Contracts;
using Swashbuckle.AspNetCore.Filters;

namespace Rentals.Api.Swagger
{
    public class SuccessOkDevolution : IExamplesProvider<ErrorResponse>
    {
        public ErrorResponse GetExamples() => new("Data de devolução informada com sucesso");
    }
}
