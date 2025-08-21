using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rentals.Api.Contracts;
using Rentals.Api.Contracts.Couriers;
using Rentals.Api.Swagger;
using Rentals.Application.Couriers.Create;
using Rentals.Application.Couriers.UploadCnh;
using Rentals.Application.Motorcycles.CreateMotorcycle;
using Swashbuckle.AspNetCore.Filters;

namespace Rentals.Api.Controllers
{
    [ApiController]
    [Route("entregadores")]
    public class CourierController  : ControllerBase
    {
        private readonly IMediator _mediator;

        public CourierController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Criar([FromBody] CreateCourierRequest body, CancellationToken ct)
        {
            var result = await _mediator.Send(new CreateCourierCommand(
                body.Identificador, body.Nome, body.Cnpj, body.DataNascimento,
                body.NumeroCnh, body.TipoCnh, body.ImagemCnh
            ), ct);

            return Created();
        }


        [HttpPost("{id}/cnh")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadCnh([FromRoute] string id, [FromBody] UploadCnhRequest request, CancellationToken ct)
        {
            var url = await _mediator.Send(new UploadCnhImageCommand(
                id, request.ImagemCnh
            ), ct);

            return Created();
        }
    }
}
