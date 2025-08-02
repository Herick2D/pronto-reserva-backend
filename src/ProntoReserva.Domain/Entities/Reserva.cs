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
    public DateTime? DeletedAt { get; private set; }
    
    private Reserva(Guid id, string nomeCliente, DateTime dataReserva, int numeroPessoas) : base(id)
    {
        NomeCliente = nomeCliente;
        DataReserva = dataReserva;
        NumeroPessoas = numeroPessoas;
        Status = StatusReserva.Pendente;
    }
    
    private Reserva() { }
    
    private static DateTime GetBrazilTimeNow()
    {
        var brazilTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");
        return TimeZoneInfo.ConvertTime(DateTime.UtcNow, brazilTimeZone);
    }
    
    private static DateTime NormalizeToUtc(DateTime dt)
    {
        return dt.Kind switch
        {
            DateTimeKind.Unspecified => DateTime.SpecifyKind(dt, DateTimeKind.Utc),
            DateTimeKind.Local => dt.ToUniversalTime(),
            _ => dt
        };
    }

    public static Reserva Criar(string nomeCliente, DateTime dataReserva, int numeroPessoas)
    {
        var dataReservaUtc = NormalizeToUtc(dataReserva);
        var agoraNoBrasil = GetBrazilTimeNow();

        if (string.IsNullOrWhiteSpace(nomeCliente))
            throw new ArgumentException("O nome do cliente não pode ser vazio.", nameof(nomeCliente));

        if (dataReservaUtc < agoraNoBrasil)
            throw new ArgumentException("A data da reserva não pode ser no passado.", nameof(dataReserva));

        if (numeroPessoas <= 0)
            throw new ArgumentException("O número de pessoas deve ser maior que zero.", nameof(numeroPessoas));

        return new Reserva(Guid.NewGuid(), nomeCliente, dataReservaUtc, numeroPessoas);
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
    
    //TODO: transformar as validações em uma função para evitar boilerplate
    public void Atualizar(string nomeCliente, DateTime dataReserva, int numeroPessoas)
    {
        var dataReservaUtc = NormalizeToUtc(dataReserva);
        var agoraNoBrasil = GetBrazilTimeNow();

        if (string.IsNullOrWhiteSpace(nomeCliente))
            throw new ArgumentException("O nome do cliente não pode ser vazio.", nameof(nomeCliente));

        if (dataReservaUtc < agoraNoBrasil)
            throw new ArgumentException("A data da reserva não pode ser no passado.", nameof(dataReserva));

        if (numeroPessoas <= 0)
            throw new ArgumentException("O número de pessoas deve ser maior que zero.", nameof(numeroPessoas));

        NomeCliente = nomeCliente;
        DataReserva = dataReservaUtc;
        NumeroPessoas = numeroPessoas;
    }
    
    public void Apagar()
    {
        var agoraNoBrasil = GetBrazilTimeNow();
        DeletedAt = agoraNoBrasil;
    }
}
