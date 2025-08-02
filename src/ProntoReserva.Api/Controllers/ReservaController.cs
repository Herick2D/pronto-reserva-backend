using Microsoft.AspNetCore.Mvc;
using ProntoReserva.Application.Features.Reservas.Commands.ConfirmarReserva;
using ProntoReserva.Application.Features.Reservas.Commands.CreateReserva;
using ProntoReserva.Application.Features.Reservas.Commands.DeleteReserva;
using ProntoReserva.Application.Features.Reservas.Commands.UpdateReserva;
using ProntoReserva.Application.Features.Reservas.Queries.GetAllReservas;
using ProntoReserva.Application.Features.Reservas.Queries.GetReservaById;

namespace ProntoReserva.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservasController : ControllerBase
{
    private readonly CreateReservaCommandHandler _createReservaCommandHandler;
    private readonly UpdateReservaCommandHandler _updateReservaCommandHandler;
    private readonly DeleteReservaCommandHandler _deleteReservaCommandHandler;
    private readonly ConfirmarReservaCommandHandler _confirmarReservaCommandHandler;
    private readonly GetReservaByIdQueryHandler _getReservaByIdQueryHandler;
    private readonly GetAllReservasQueryHandler _getAllReservasQueryHandler;

    public ReservasController(
        CreateReservaCommandHandler createReservaCommandHandler,
        UpdateReservaCommandHandler updateReservaCommandHandler,
        DeleteReservaCommandHandler deleteReservaCommandHandler,
        ConfirmarReservaCommandHandler confirmarReservaCommandHandler,
        GetReservaByIdQueryHandler getReservaByIdQueryHandler,
        GetAllReservasQueryHandler getAllReservasQueryHandler)
    {
        _createReservaCommandHandler = createReservaCommandHandler;
        _updateReservaCommandHandler = updateReservaCommandHandler;
        _deleteReservaCommandHandler = deleteReservaCommandHandler;
        _confirmarReservaCommandHandler = confirmarReservaCommandHandler;
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
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReserva(Guid id, [FromBody] UpdateReservaCommand command)
    {
        //TODO essa validação me parece desnecessária, definir se ela faz sentido. 
        if (id != command.Id)
        {
            return BadRequest("O ID da rota não corresponde ao ID do corpo da requisição.");
        }

        try
        {
            await _updateReservaCommandHandler.Handle(command);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Ocorreu um erro inesperado.", details = ex.Message });
        }
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReserva(Guid id)
    {
        try
        {
            var command = new DeleteReservaCommand(id);
            await _deleteReservaCommandHandler.Handle(command);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Ocorreu um erro inesperado.", details = ex.Message });
        }
    }
    
    [HttpPost("{id}/confirmar")]
    public async Task<IActionResult> ConfirmarReserva(Guid id)
    {
        try
        {
            var command = new ConfirmarReservaCommand(id);
            await _confirmarReservaCommandHandler.Handle(command);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Ocorreu um erro inesperado.", details = ex.Message });
        }
    }
    
}
