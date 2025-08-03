using Dapper;
using Npgsql;
using ProntoReserva.Domain.Entities;
using ProntoReserva.Domain.Repositories;
using System.Data;

namespace ProntoReserva.Infrastructure.Repositories;

public class ReservaRepository : IReservaRepository
{
    private readonly string _connectionString;

    public ReservaRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    private IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);

    public async Task<Reserva?> GetByIdAsync(Guid id, Guid userId)
    {
        const string sql = "SELECT * FROM \"Reservas\" WHERE \"Id\" = @Id AND \"UserId\" = @UserId AND \"DeletedAt\" IS NULL";
        using var connection = CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<Reserva>(sql, new { Id = id, UserId = userId });
    }

    public async Task<(ICollection<Reserva> Reservas, int TotalCount)> GetAllAsync(int pageNumber, int pageSize, Guid userId)
    {
        const string sql = @"
            SELECT COUNT(*) FROM ""Reservas"" WHERE ""UserId"" = @UserId AND ""DeletedAt"" IS NULL;

            SELECT * FROM ""Reservas""
            WHERE ""UserId"" = @UserId AND ""DeletedAt"" IS NULL
            ORDER BY ""DataReserva"" DESC
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY;
        ";

        using var connection = CreateConnection();
        using var multi = await connection.QueryMultipleAsync(sql, new 
        { 
            UserId = userId,
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
            INSERT INTO ""Reservas"" (""Id"", ""NomeCliente"", ""DataReserva"", ""NumeroPessoas"", ""Status"", ""Observacoes"", ""DeletedAt"", ""UserId"")
            VALUES (@Id, @NomeCliente, @DataReserva, @NumeroPessoas, @Status, @Observacoes, @DeletedAt, @UserId)";
        using var connection = CreateConnection();
        await connection.ExecuteAsync(sql, reserva);
    }

    public async Task UpdateAsync(Reserva reserva)
    {
        const string sql = @"
            UPDATE ""Reservas""
            SET ""NomeCliente"" = @NomeCliente,
                ""DataReserva"" = @DataReserva,
                ""NumeroPessoas"" = @NumeroPessoas,
                ""Status"" = @Status,
                ""Observacoes"" = @Observacoes,
                ""DeletedAt"" = @DeletedAt 
            WHERE ""Id"" = @Id AND ""UserId"" = @UserId";
        using var connection = CreateConnection();
        await connection.ExecuteAsync(sql, reserva);
    }
}
