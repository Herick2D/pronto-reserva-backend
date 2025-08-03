using ProntoReserva.Application.Abstractions.Authentication;
using ProntoReserva.Application.Abstractions.Messaging;
using ProntoReserva.Application.Events;
using ProntoReserva.Domain.Repositories;

namespace ProntoReserva.Application.Features.Reservas.Commands.ConfirmarReserva;

public class ConfirmarReservaCommandHandler
{
    private readonly IReservaRepository _reservaRepository;
    private readonly IMessagePublisher _messagePublisher;
    private readonly IUserContext _userContext;

    public ConfirmarReservaCommandHandler(
        IReservaRepository reservaRepository,
        IMessagePublisher messagePublisher,
        IUserContext userContext)
    {
        _reservaRepository = reservaRepository;
        _messagePublisher = messagePublisher;
        _userContext = userContext;
    }

    public async Task Handle(ConfirmarReservaCommand command)
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

        reserva.Confirmar();

        await _reservaRepository.UpdateAsync(reserva);

        var evento = new ReservaConfirmadaEvent(reserva.Id);
        await _messagePublisher.PublishAsync(evento);
    }
}