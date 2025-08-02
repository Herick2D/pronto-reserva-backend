using Microsoft.AspNetCore.Mvc;
using ProntoReserva.Application.Features.Reservas.Commands.CreateReserva;

namespace ProntoReserva.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservasController : ControllerBase
{
    private readonly CreateReservaCommandHandler _createReservaCommandHandler;

    public ReservasController(CreateReservaCommandHandler createReservaCommandHandler)
    {
        _createReservaCommandHandler = createReservaCommandHandler;
    }

    [HttpPost]
    public async Task<IActionResult> CreateReserva([FromBody] CreateReservaCommand command)
    {
        try
        {
            var reservaId = await _createReservaCommandHandler.Handle(command);
            
            return CreatedAtAction(nameof(GetReservaById), new { id = reservaId }, new { id = reservaId });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {

            return StatusCode(500, new { message = "Ocorreu um erro inesperado ao processar sua solicitação.", details = ex.Message });
        }
    }
    
    [HttpGet("{id}")]
    public IActionResult GetReservaById(Guid id)
    {
        return Ok(new { Message = $"Endpoint para buscar a reserva {id} será implementado em breve." });
    }
}
