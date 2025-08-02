namespace ProntoReserva.Application.Features.Reservas.Commands.UpdateReserva;

public record UpdateReservaCommand(
    Guid Id,
    string NomeCliente,
    DateTime DataReserva,
    int NumeroPessoas,
    string? Observacoes
);