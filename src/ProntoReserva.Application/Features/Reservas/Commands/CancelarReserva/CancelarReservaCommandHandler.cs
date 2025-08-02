using ProntoReserva.Domain.Repositories;

namespace ProntoReserva.Application.Features.Reservas.Commands.CancelarReserva;

public class CancelarReservaCommandHandler
{
    private readonly IReservaRepository _reservaRepository;

    public CancelarReservaCommandHandler(IReservaRepository reservaRepository)
    {
        _reservaRepository = reservaRepository;
    }

    public async Task Handle(CancelarReservaCommand command)
    {
        var reserva = await _reservaRepository.GetByIdAsync(command.Id);
        if (reserva is null)
        {
            throw new KeyNotFoundException($"Reserva com o ID {command.Id} não encontrada.");
        }

        reserva.Cancelar();

        await _reservaRepository.UpdateAsync(reserva);
    }
}