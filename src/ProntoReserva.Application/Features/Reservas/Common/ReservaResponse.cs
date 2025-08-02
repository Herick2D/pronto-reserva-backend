namespace ProntoReserva.Application.Features.Reservas.Common;

public class ReservaResponse
{
    public Guid Id { get; set; }
    public string NomeCliente { get; set; } = string.Empty;
    public DateTime DataReserva { get; set; }
    public int NumeroPessoas { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Observacoes { get; set; }
}