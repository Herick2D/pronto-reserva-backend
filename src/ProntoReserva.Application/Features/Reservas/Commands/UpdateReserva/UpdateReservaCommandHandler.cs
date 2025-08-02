using ProntoReserva.Domain.Repositories;

namespace ProntoReserva.Application.Features.Reservas.Commands.UpdateReserva;

public class UpdateReservaCommandHandler
{
    private readonly IReservaRepository _reservaRepository;

    public UpdateReservaCommandHandler(IReservaRepository reservaRepository)
    {
        _reservaRepository = reservaRepository;
    }

    public async Task Handle(UpdateReservaCommand command)
    {
        var reserva = await _reservaRepository.GetByIdAsync(command.Id);
        
        if (reserva is null)
        {
            throw new KeyNotFoundException($"Reserva com o ID {command.Id} não encontrada.");
        }

        reserva.Atualizar(command.NomeCliente, command.DataReserva, command.NumeroPessoas);

        if (command.Observacoes is not null)
        {
            reserva.AdicionarObservacoes(command.Observacoes);
        }

        await _reservaRepository.UpdateAsync(reserva);
    }
}