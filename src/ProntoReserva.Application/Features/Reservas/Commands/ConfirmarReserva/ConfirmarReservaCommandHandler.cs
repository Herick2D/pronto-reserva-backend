using ProntoReserva.Application.Abstractions.Messaging;
using ProntoReserva.Domain.Repositories;
using ProntoReserva.Application.Events;


namespace ProntoReserva.Application.Features.Reservas.Commands.ConfirmarReserva;

public class ConfirmarReservaCommandHandler
{
    private readonly IReservaRepository _reservaRepository;
    private readonly IMessagePublisher _messagePublisher;

    public ConfirmarReservaCommandHandler(
        IReservaRepository reservaRepository,
        IMessagePublisher messagePublisher
        )
    {
        _reservaRepository = reservaRepository;
        _messagePublisher = messagePublisher;
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

        var eventoConfirmacao = new ReservaConfirmadaEvent(reserva.Id);
        await _messagePublisher.PublishAsync(eventoConfirmacao);

        await PublicarLembreteAgendado(reserva);
    }
    
    private async Task PublicarLembreteAgendado(Domain.Entities.Reserva reserva)
    {
        var dataLembrete = reserva.DataReserva.AddDays(-1);
        if (dataLembrete <= DateTime.UtcNow)
        {
            return;
        }

        var ttl = (int)(dataLembrete - DateTime.UtcNow).TotalMilliseconds;
        
        var eventoLembrete = new LembreteReservaEvent(reserva.Id, reserva.NomeCliente);

        await _messagePublisher.PublishWithDelayAsync(eventoLembrete, ttl);
    }
}