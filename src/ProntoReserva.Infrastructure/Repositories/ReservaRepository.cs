using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using ProntoReserva.Domain.Entities;
using ProntoReserva.Domain.Repositories;

namespace ProntoReserva.Infrastructure.Repositories;

public class ReservaRepository : IReservaRepository
{
    private readonly string _connectionString;

    public ReservaRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

    public async Task<Reserva?> GetByIdAsync(Guid id)
    {
        const string sql = "SELECT * FROM Reservas WHERE Id = @Id";
        using var connection = CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<Reserva>(sql, new { Id = id });
    }

    public async Task<ICollection<Reserva>> GetAllAsync()
    {
        const string sql = "SELECT * FROM Reservas";
        using var connection = CreateConnection();
        var reservas = await connection.QueryAsync<Reserva>(sql);
        return reservas.ToList();
    }

    public async Task AddAsync(Reserva reserva)
    {
        const string sql = @"
            INSERT INTO Reservas (Id, NomeCliente, DataReserva, NumeroPessoas, Status, Observacoes)
            VALUES (@Id, @NomeCliente, @DataReserva, @NumeroPessoas, @Status, @Observacoes)";
        using var connection = CreateConnection();
        await connection.ExecuteAsync(sql, reserva);
    }

    public async Task UpdateAsync(Reserva reserva)
    {
        const string sql = @"
            UPDATE Reservas
            SET NomeCliente = @NomeCliente,
                DataReserva = @DataReserva,
                NumeroPessoas = @NumeroPessoas,
                Status = @Status,
                Observacoes = @Observacoes
            WHERE Id = @Id";
        using var connection = CreateConnection();
        await connection.ExecuteAsync(sql, reserva);
    }

    public async Task DeleteAsync(Guid id)
    {
        const string sql = "DELETE FROM Reservas WHERE Id = @Id";
        using var connection = CreateConnection();
        await connection.ExecuteAsync(sql, new { Id = id });
    }
}
