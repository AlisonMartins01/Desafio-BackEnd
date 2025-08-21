using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Rentals.Api.Contracts;
using Rentals.Api.Contracts.Motorcycle;
using Rentals.Api.Swagger;
using Rentals.Application.Motorcycles;
using Rentals.Application.Motorcycles.ChangePlate;
using Rentals.Application.Motorcycles.CreateMotorcycle;
using Rentals.Application.Motorcycles.Delete;
using Rentals.Application.Motorcycles.GetByIdentifier;
using Rentals.Application.Motorcycles.GetByPlate;
using Swashbuckle.AspNetCore.Filters;

namespace Rentals.Api.Controllers
{
    [ApiController]
    [Route("motos")]
    public class MotorcyclesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MotorcyclesController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ErroResponseExample))]
        public async Task<IActionResult> Create([FromBody] CreateMotorcycleCommand cmd, CancellationToken ct)
        {
            var result = await _mediator.Send(cmd, ct);
            return Created();
        }

        [HttpGet()]
        [ProducesResponseType(typeof(MotorcycleResponse[]), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByPlate([FromQuery] string placa, CancellationToken ct)
        {
            var dto = await _mediator.Send(new GetMotorcycleByPlateQuery(placa), ct);
            var resp = new MotorcycleResponse(dto.Identifier, dto.Year, dto.Model, dto.Plate);
            var respList = new List<MotorcycleResponse>();
            respList.Add(resp);
            return Ok(respList);
        }


        [HttpPut("{id}/placa")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(ChangePlateResponseExample), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        
        [SwaggerRequestExample(typeof(ChangePlateRequest), typeof(ChangePlateRequestExample))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ChangePlateResponseExample))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ErroResponseExample))]
        public async Task<IActionResult> ChangePlate([FromRoute] string id, [Required]ChangePlateRequest body, CancellationToken ct)
        {
            await _mediator.Send(new ChangeMotorcyclePlateCommand(id, body.NewPlate), ct);
            return Ok(new ChangePlateResponse());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MotorcycleResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ErrorRequestFormatExample))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ErrorNotFoundMotorcycleExample))]

        public async Task<IActionResult> GetByIdentifier([FromRoute] string id, CancellationToken ct)
        {
            var dto = await _mediator.Send(new GetMotorcycleByIdentifierQuery(id), ct);
            var resp = new MotorcycleResponse(dto.Identifier, dto.Year, dto.Model, dto.Plate);
            return Ok(resp);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ErroResponseExample))]
        public async Task<IActionResult> Delete(string id, CancellationToken ct)
        {
            await _mediator.Send(new DeleteMotorcycleCommand(id), ct);
            return Ok();
        }
    }
}
