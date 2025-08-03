using Dapper;
using Microsoft.Data.SqlClient;
using ProntoReserva.Domain.Entities;
using ProntoReserva.Domain.Repositories;
using System.Data;

namespace ProntoReserva.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly string _connectionString;

    public UserRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

    public async Task<User?> GetByEmailAsync(string email)
    {
        const string sql = "SELECT * FROM Users WHERE Email = @Email";
        using var connection = CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<User>(sql, new { Email = email });
    }

    public async Task AddAsync(User user)
    {
        const string sql = "INSERT INTO Users (Id, Email, PasswordHash) VALUES (@Id, @Email, @PasswordHash)";
        using var connection = CreateConnection();
        await connection.ExecuteAsync(sql, user);
    }
}