using ProntoReserva.Application.Abstractions.Authentication;
using ProntoReserva.Application.Features.Reservas.Common;
using ProntoReserva.Domain.Repositories;

namespace ProntoReserva.Application.Features.Reservas.Queries.GetReservaById;

public class GetReservaByIdQueryHandler
{
    private readonly IReservaRepository _reservaRepository;
    private readonly IUserContext _userContext;

    public GetReservaByIdQueryHandler(
        IReservaRepository reservaRepository, 
        IUserContext userContext)
    {
        _reservaRepository = reservaRepository;
        _userContext = userContext;
    }

    public async Task<ReservaResponse?> Handle(Guid id)
    {
        var userId = _userContext.GetUserId();
        if (userId is null)
        {
            throw new UnauthorizedAccessException("Utilizador não autenticado.");
        }

        var reserva = await _reservaRepository.GetByIdAsync(id, userId.Value);

        if (reserva is null)
        {
            return null;
        }

        return new ReservaResponse
        {
            Id = reserva.Id,
            NomeCliente = reserva.NomeCliente,
            DataReserva = reserva.DataReserva,
            NumeroPessoas = reserva.NumeroPessoas,
            Status = reserva.Status.ToString(),
            Observacoes = reserva.Observacoes
        };
    }
}