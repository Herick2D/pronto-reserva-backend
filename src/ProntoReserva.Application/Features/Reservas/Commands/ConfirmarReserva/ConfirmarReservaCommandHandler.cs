using ProntoReserva.Domain.Repositories;


namespace ProntoReserva.Application.Features.Reservas.Commands.ConfirmarReserva;

public class ConfirmarReservaCommandHandler
{
    private readonly IReservaRepository _reservaRepository;

    public ConfirmarReservaCommandHandler(IReservaRepository reservaRepository)
    {
        _reservaRepository = reservaRepository;
    }

    public async Task Handle(ConfirmarReservaCommand command)
    {
        var reserva = await _reservaRepository.GetByIdAsync(command.Id);
        if (reserva is null)
        {
            throw new KeyNotFoundException($"Reserva com o ID {command.Id} não encontrada.");
        }

        reserva.Confirmar();
        await _reservaRepository.UpdateAsync(reserva);
    }
}