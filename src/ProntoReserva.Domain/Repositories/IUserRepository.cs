using ProntoReserva.Domain.Entities;

namespace ProntoReserva.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task AddAsync(User user);
}