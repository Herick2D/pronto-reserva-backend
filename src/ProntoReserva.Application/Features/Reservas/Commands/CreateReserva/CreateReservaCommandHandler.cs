using ProntoReserva.Application.Abstractions.Authentication;
using ProntoReserva.Domain.Entities;
using ProntoReserva.Domain.Repositories;


namespace ProntoReserva.Application.Features.Reservas.Commands.CreateReserva;

public class CreateReservaCommandHandler
{
    private readonly IReservaRepository _reservaRepository;
    private readonly IUserContext _userContext;

    public CreateReservaCommandHandler(
        IReservaRepository reservaRepository, 
        IUserContext userContext)
    {
        _reservaRepository = reservaRepository;
        _userContext = userContext;
    }

    public async Task<Guid> Handle(CreateReservaCommand command)
    {
        var userId = _userContext.GetUserId();
        if (userId is null)
        {
            throw new UnauthorizedAccessException("Utilizador não autenticado.");
        }

        var reserva = Reserva.Criar(
            command.NomeCliente,
            command.DataReserva,
            command.NumeroPessoas,
            userId.Value
        );
        
        if (!string.IsNullOrWhiteSpace(command.Observacoes))
        {
            reserva.AdicionarObservacoes(command.Observacoes);
        }

        await _reservaRepository.AddAsync(reserva);

        return reserva.Id;
    }
}