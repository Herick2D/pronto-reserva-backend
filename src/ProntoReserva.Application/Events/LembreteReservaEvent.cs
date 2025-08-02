namespace ProntoReserva.Application.Events;

public record class LembreteReservaEvent(Guid ReservaId, string NomeCliente);