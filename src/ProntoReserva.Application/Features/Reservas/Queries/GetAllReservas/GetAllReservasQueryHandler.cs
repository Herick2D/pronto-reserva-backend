using ProntoReserva.Application.Features.Reservas.Common;
using ProntoReserva.Domain.Repositories;

namespace ProntoReserva.Application.Features.Reservas.Queries.GetAllReservas;

public class GetAllReservasQueryHandler
{
    private readonly IReservaRepository _reservaRepository;

    public GetAllReservasQueryHandler(IReservaRepository reservaRepository)
    {
        _reservaRepository = reservaRepository;
    }

    public async Task<PaginatedResponse<ReservaResponse>> Handle(GetAllReservasQuery query)
    {

        var (reservas, totalCount) = await _reservaRepository.GetAllAsync(query.PageNumber, query.PageSize);

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