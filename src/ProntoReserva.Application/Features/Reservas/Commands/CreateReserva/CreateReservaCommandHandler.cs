using ProntoReserva.Domain.Entities;
using ProntoReserva.Domain.Repositories;

namespace ProntoReserva.Application.Features.Reservas.Commands.CreateReserva;

public class CreateReservaCommandHandler
{
    private readonly IReservaRepository _reservaRepository;

    public CreateReservaCommandHandler(IReservaRepository reservaRepository)
    {
        _reservaRepository = reservaRepository;
    }

    public async Task<Guid> Handle(CreateReservaCommand command)
    {

        var reserva = Reserva.Criar(
            command.NomeCliente,
            command.DataReserva,
            command.NumeroPessoas
        );
        
        if (!string.IsNullOrWhiteSpace(command.Observacoes))
        {
            reserva.AdicionarObservacoes(command.Observacoes);
        }

        await _reservaRepository.AddAsync(reserva);

        return reserva.Id;
    }
}