using ProntoReserva.Domain.Entities;

namespace ProntoReserva.Domain.Repositories;

public interface IReservaRepository
{
    Task<Reserva?> GetByIdAsync(Guid id, Guid userId);

    Task<(ICollection<Reserva> Reservas, int TotalCount)> GetAllAsync(int pageNumber, int pageSize, Guid userId);
    
    Task AddAsync(Reserva reserva);

    Task UpdateAsync(Reserva reserva);
}