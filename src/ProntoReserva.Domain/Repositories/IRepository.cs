using ProntoReserva.Domain.Entities;

namespace ProntoReserva.Domain.Repositories;

public interface IReservaRepository
{
    Task<Reserva?> GetByIdAsync(Guid id);

    Task<(ICollection<Reserva> Reservas, int TotalCount)> GetAllAsync(int pageNumber, int pageSize);
    
    Task AddAsync(Reserva reserva);

    Task UpdateAsync(Reserva reserva);

    Task DeleteAsync(Guid id);
}