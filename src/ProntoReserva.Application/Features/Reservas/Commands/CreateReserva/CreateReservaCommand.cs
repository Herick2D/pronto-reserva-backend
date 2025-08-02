namespace ProntoReserva.Application.Features.Reservas.Commands.CreateReserva;

public record CreateReservaCommand(
    string NomeCliente,
    DateTime DataReserva,
    int NumeroPessoas,
    string? Observacoes
);