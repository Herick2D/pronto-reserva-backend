using ProntoReserva.Application.Features.Reservas.Common;
using ProntoReserva.Domain.Repositories;

namespace ProntoReserva.Application.Features.Reservas.Queries.GetReservaById;

public class GetReservaByIdQueryHandler
{
    private readonly IReservaRepository _reservaRepository;

    public GetReservaByIdQueryHandler(IReservaRepository reservaRepository)
    {
        _reservaRepository = reservaRepository;
    }

    public async Task<ReservaResponse?> Handle(Guid id)
    {
        var reserva = await _reservaRepository.GetByIdAsync(id);

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