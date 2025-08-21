using Rentals.Api.Contracts.Motorcycle;
using Swashbuckle.AspNetCore.Filters;

namespace Rentals.Api.Swagger
{
    public sealed class ChangePlateRequestExample : IExamplesProvider<ChangePlateRequest>
    {
        public ChangePlateRequest GetExamples() => new ChangePlateRequest() { NewPlate = "ABC-1234"};
    }
}
