using Microsoft.AspNetCore.Mvc;
using ProntoReserva.Application.Features.Reservas.Commands.CreateReserva;
using ProntoReserva.Application.Features.Reservas.Queries.GetAllReservas;
using ProntoReserva.Application.Features.Reservas.Queries.GetReservaById;

namespace ProntoReserva.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservasController : ControllerBase
{
    private readonly CreateReservaCommandHandler _createReservaCommandHandler;
    private readonly GetReservaByIdQueryHandler _getReservaByIdQueryHandler;
    private readonly GetAllReservasQueryHandler _getAllReservasQueryHandler;

    public ReservasController(
        CreateReservaCommandHandler createReservaCommandHandler,
        GetReservaByIdQueryHandler getReservaByIdQueryHandler,
        GetAllReservasQueryHandler getAllReservasQueryHandler)
    {
        _createReservaCommandHandler = createReservaCommandHandler;
        _getReservaByIdQueryHandler = getReservaByIdQueryHandler;
        _getAllReservasQueryHandler = getAllReservasQueryHandler;
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
    public async Task<IActionResult> GetReservaById(Guid id)
    {
        var reserva = await _getReservaByIdQueryHandler.Handle(id);

        return reserva is not null ? Ok(reserva) : NotFound();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllReservas([FromQuery] GetAllReservasQuery query)
    {
        var result = await _getAllReservasQueryHandler.Handle(query);
        return Ok(result);
    }
}
