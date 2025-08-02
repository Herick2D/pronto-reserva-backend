using FluentAssertions;
using Moq;
using ProntoReserva.Application.Features.Reservas.Commands.UpdateReserva;
using ProntoReserva.Domain.Entities;
using ProntoReserva.Domain.Repositories;

namespace ProntoReserva.Tests.Unit.Application.Features.Reservas.Commands.UpdateReserva;

public class UpdateReservaCommandHandlerTests
{
    private readonly Mock<IReservaRepository> _mockReservaRepository;

    public UpdateReservaCommandHandlerTests()
    {
        _mockReservaRepository = new Mock<IReservaRepository>();
    }

    [Fact]
    public async Task Handle_QuandoReservaExiste_DeveChamarUpdateAsyncNoRepositorio()
    {
        var reservaId = Guid.NewGuid();
        var reservaExistente = Reserva.Criar("Cliente Antigo", DateTime.UtcNow.AddDays(2), 2);

        _mockReservaRepository
            .Setup(repo => repo.GetByIdAsync(reservaId))
            .ReturnsAsync(reservaExistente);
            
        var handler = new UpdateReservaCommandHandler(_mockReservaRepository.Object);
        var command = new UpdateReservaCommand(reservaId, "Cliente Novo", DateTime.UtcNow.AddDays(3), 3, null);

        await handler.Handle(command);

        _mockReservaRepository.Verify(
            repo => repo.UpdateAsync(It.Is<Reserva>(r => r.NomeCliente == "Cliente Novo")), 
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
            
        var handler = new UpdateReservaCommandHandler(_mockReservaRepository.Object);
        var command = new UpdateReservaCommand(idInexistente, "Cliente", DateTime.UtcNow.AddDays(1), 1, null);

        Func<Task> act = async () => await handler.Handle(command);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
