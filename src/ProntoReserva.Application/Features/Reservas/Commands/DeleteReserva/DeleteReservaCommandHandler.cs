using ProntoReserva.Domain.Repositories;

namespace ProntoReserva.Application.Features.Reservas.Commands.DeleteReserva;

public class DeleteReservaCommandHandler
{
    private readonly IReservaRepository _reservaRepository;

    public DeleteReservaCommandHandler(IReservaRepository reservaRepository)
    {
        _reservaRepository = reservaRepository;
    }

    public async Task Handle(DeleteReservaCommand command)
    {
        var reserva = await _reservaRepository.GetByIdAsync(command.Id);
        if (reserva is null)
        {
            throw new KeyNotFoundException($"Reserva com o ID {command.Id} não encontrada.");
        }
        
        await _reservaRepository.DeleteAsync(command.Id);
    }
}