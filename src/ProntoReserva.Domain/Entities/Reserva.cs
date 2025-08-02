using ProntoReserva.Domain.Common;
using ProntoReserva.Domain.Enums;

namespace ProntoReserva.Domain.Entities;

public class Reserva : Entity
{
    public string NomeCliente { get; private set; }
    public DateTime DataReserva { get; private set; }
    public int NumeroPessoas { get; private set; }
    public StatusReserva Status { get; private set; }
    public string? Observacoes { get; private set; }
    
    private Reserva(Guid id, string nomeCliente, DateTime dataReserva, int numeroPessoas) : base(id)
    {
        NomeCliente = nomeCliente;
        DataReserva = dataReserva;
        NumeroPessoas = numeroPessoas;
        Status = StatusReserva.Pendente;
    }
    
    private Reserva() { }

    public static Reserva Criar(string nomeCliente, DateTime dataReserva, int numeroPessoas)
    {
        if (string.IsNullOrWhiteSpace(nomeCliente))
        {
            throw new ArgumentException("O nome do cliente não pode ser vazio.", nameof(nomeCliente));
        }

        if (dataReserva < DateTime.UtcNow)
        {
            throw new ArgumentException("A data da reserva não pode ser no passado.", nameof(dataReserva));
        }

        if (numeroPessoas <= 0)
        {
            throw new ArgumentException("O número de pessoas deve ser maior que zero.", nameof(numeroPessoas));
        }

        return new Reserva(Guid.NewGuid(), nomeCliente, dataReserva, numeroPessoas);
    }
    
    public void Confirmar()
    {
        if (Status != StatusReserva.Pendente)
        {
            throw new InvalidOperationException("Apenas reservas pendentes podem ser confirmadas.");
        }
        Status = StatusReserva.Confirmada;
    }

    public void Cancelar()
    {
        if (Status == StatusReserva.Concluida)
        {
            throw new InvalidOperationException("Não é possível cancelar uma reserva já concluída.");
        }
        Status = StatusReserva.Cancelada;
    }
    
    public void AdicionarObservacoes(string observacoes)
    {
        Observacoes = observacoes;
    }
}
