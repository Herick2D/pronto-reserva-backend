using ProntoReserva.Application.Abstractions.Authentication;
using ProntoReserva.Domain.Repositories;

namespace ProntoReserva.Application.Features.Reservas.Commands.CancelarReserva;

public class CancelarReservaCommandHandler
{
    private readonly IReservaRepository _reservaRepository;
    private readonly IUserContext _userContext;

    public CancelarReservaCommandHandler(
        IReservaRepository reservaRepository, 
        IUserContext userContext)
    {
        _reservaRepository = reservaRepository;
        _userContext = userContext;
    }

    public async Task Handle(CancelarReservaCommand command)
    {
        var userId = _userContext.GetUserId();
        if (userId is null)
        {
            throw new UnauthorizedAccessException("Utilizador não autenticado.");
        }

        var reserva = await _reservaRepository.GetByIdAsync(command.Id, userId.Value);
        if (reserva is null)
        {
            throw new KeyNotFoundException($"Reserva com o ID {command.Id} não encontrada.");
        }

        reserva.Cancelar();

        await _reservaRepository.UpdateAsync(reserva);
    }
}