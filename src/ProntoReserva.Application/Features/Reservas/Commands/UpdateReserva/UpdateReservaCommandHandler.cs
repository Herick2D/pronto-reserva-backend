using ProntoReserva.Application.Abstractions.Authentication;
using ProntoReserva.Domain.Repositories;

namespace ProntoReserva.Application.Features.Reservas.Commands.UpdateReserva;

public class UpdateReservaCommandHandler
{
    private readonly IReservaRepository _reservaRepository;
    private readonly IUserContext _userContext;

    public UpdateReservaCommandHandler(
        IReservaRepository reservaRepository, 
        IUserContext userContext)
    {
        _reservaRepository = reservaRepository;
        _userContext = userContext;
    }

    public async Task Handle(UpdateReservaCommand command)
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

        reserva.Atualizar(command.NomeCliente, command.DataReserva, command.NumeroPessoas);
        
        if (command.Observacoes is not null)
        {
            reserva.AdicionarObservacoes(command.Observacoes);
        }

        await _reservaRepository.UpdateAsync(reserva);
    }
}