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
        const string sql = "SELECT * FROM Reservas WHERE Id = @Id AND DeletedAt IS NULL";
        using var connection = CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<Reserva>(sql, new { Id = id });
    }

    public async Task<(ICollection<Reserva> Reservas, int TotalCount)> GetAllAsync(int pageNumber, int pageSize)
    {
        const string sql = @"
            -- Apenas conta os registos ativos
            SELECT COUNT(*) FROM Reservas WHERE DeletedAt IS NULL;

            SELECT * FROM Reservas
            WHERE DeletedAt IS NULL -- Adicionado para soft delete
            ORDER BY DataReserva DESC
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY;
        ";

        using var connection = CreateConnection();
        using var multi = await connection.QueryMultipleAsync(sql, new
        {
            Offset = (pageNumber - 1) * pageSize,
            PageSize = pageSize
        });

        var totalCount = await multi.ReadSingleAsync<int>();
        var reservas = (await multi.ReadAsync<Reserva>()).ToList();

        return (reservas, totalCount);
    }

    public async Task AddAsync(Reserva reserva)
    {
        const string sql = @"
            INSERT INTO Reservas (Id, NomeCliente, DataReserva, NumeroPessoas, Status, Observacoes, DeletedAt)
            VALUES (@Id, @NomeCliente, @DataReserva, @NumeroPessoas, @Status, @Observacoes, @DeletedAt)";
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
                Observacoes = @Observacoes,
                DeletedAt = @DeletedAt 
            WHERE Id = @Id";
        using var connection = CreateConnection();
        await connection.ExecuteAsync(sql, reserva);
    }
}
