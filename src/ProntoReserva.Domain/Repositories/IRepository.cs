using ProntoReserva.Domain.Entities;

namespace ProntoReserva.Domain.Repositories;

public interface IReservaRepository
{

    Task<Reserva?> GetByIdAsync(Guid id);

    Task<ICollection<Reserva>> GetAllAsync();

    Task AddAsync(Reserva reserva);

    Task UpdateAsync(Reserva reserva);

    Task DeleteAsync(Guid id);
}