using FluentAssertions;
using Moq;
using ProntoReserva.Application.Features.Reservas.Commands.DeleteReserva;
using ProntoReserva.Domain.Entities;
using ProntoReserva.Domain.Repositories;

namespace ProntoReserva.Tests.Unit.Application.Features.Reservas.Commands.DeleteReserva;

public class DeleteReservaCommandHandlerTests
{
    private readonly Mock<IReservaRepository> _mockReservaRepository;

    public DeleteReservaCommandHandlerTests()
    {
        _mockReservaRepository = new Mock<IReservaRepository>();
    }

    [Fact]
    public async Task Handle_QuandoReservaExiste_DeveChamarDeleteAsyncNoRepositorio()
    {
        var reservaId = Guid.NewGuid();
        var reservaExistente = Reserva.Criar("Cliente Para Apagar", DateTime.UtcNow.AddDays(1), 1);

        _mockReservaRepository
            .Setup(repo => repo.GetByIdAsync(reservaId))
            .ReturnsAsync(reservaExistente);
            
        var handler = new DeleteReservaCommandHandler(_mockReservaRepository.Object);
        var command = new DeleteReservaCommand(reservaId);

        await handler.Handle(command);

        _mockReservaRepository.Verify(
            repo => repo.DeleteAsync(reservaId), 
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_QuandoReservaNaoExiste_DeveLancarKeyNotFoundException()
    {
        var idInexistente = Guid.NewGuid();

        _mockReservaRepository
            .Setup(repo => repo.GetByIdAsync(idInexistente))
            .ReturnsAsync((Reserva?)null);
            
        var handler = new DeleteReservaCommandHandler(_mockReservaRepository.Object);
        var command = new DeleteReservaCommand(idInexistente);

        Func<Task> act = async () => await handler.Handle(command);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
