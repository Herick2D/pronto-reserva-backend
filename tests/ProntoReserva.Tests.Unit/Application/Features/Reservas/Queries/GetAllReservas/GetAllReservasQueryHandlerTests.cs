using ProntoReserva.Application.Abstractions.Authentication;
using ProntoReserva.Application.Features.Reservas.Common;
using ProntoReserva.Domain.Repositories;


namespace ProntoReserva.Application.Features.Reservas.Queries.GetAllReservas;

public class GetAllReservasQueryHandler
{
    private readonly IReservaRepository _reservaRepository;
    private readonly IUserContext _userContext;

    public GetAllReservasQueryHandler(
        IReservaRepository reservaRepository, 
        IUserContext userContext)
    {
        _reservaRepository = reservaRepository;
        _userContext = userContext;
    }

    public async Task<PaginatedResponse<ReservaResponse>> Handle(GetAllReservasQuery query)
    {
        var userId = _userContext.GetUserId();
        if (userId is null)
        {
            throw new UnauthorizedAccessException("Utilizador não autenticado.");
        }

        var (reservas, totalCount) = await _reservaRepository.GetAllAsync(query.PageNumber, query.PageSize, userId.Value);

        var reservaResponses = reservas.Select(reserva => new ReservaResponse
        {
            Id = reserva.Id,
            NomeCliente = reserva.NomeCliente,
            DataReserva = reserva.DataReserva,
            NumeroPessoas = reserva.NumeroPessoas,
            Status = reserva.Status.ToString(),
            Observacoes = reserva.Observacoes
        }).ToList();
        
        return new PaginatedResponse<ReservaResponse>(
            reservaResponses, 
            totalCount, 
            query.PageNumber, 
            query.PageSize);
    }
}