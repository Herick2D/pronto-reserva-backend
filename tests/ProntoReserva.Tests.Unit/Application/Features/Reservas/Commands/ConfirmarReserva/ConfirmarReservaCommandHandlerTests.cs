using FluentAssertions;
using Moq;
using ProntoReserva.Application.Abstractions.Messaging;
using ProntoReserva.Application.Events;
using ProntoReserva.Application.Features.Reservas.Commands.ConfirmarReserva;
using ProntoReserva.Domain.Entities;
using ProntoReserva.Domain.Enums;
using ProntoReserva.Domain.Repositories;

namespace ProntoReserva.Tests.Unit.Application.Features.Reservas.Commands.ConfirmarReserva;

public class ConfirmarReservaCommandHandlerTests
{
    private readonly Mock<IReservaRepository> _mockReservaRepository;
    private readonly Mock<IMessagePublisher> _mockMessagePublisher;

    public ConfirmarReservaCommandHandlerTests()
    {
        _mockReservaRepository = new Mock<IReservaRepository>();
        _mockMessagePublisher = new Mock<IMessagePublisher>();
    }

    [Fact]
    public async Task Handle_QuandoReservaExisteEPendente_DeveConfirmarEChamarUpdateEPublish()
    {
        var reservaId = Guid.NewGuid();
        var reservaPendente = Reserva.Criar("Cliente Teste", DateTime.UtcNow.AddDays(1), 2);
        
        _mockReservaRepository
            .Setup(repo => repo.GetByIdAsync(reservaId))
            .ReturnsAsync(reservaPendente);

        var handler = new ConfirmarReservaCommandHandler(
            _mockReservaRepository.Object, 
            _mockMessagePublisher.Object
        );
        var command = new ConfirmarReservaCommand(reservaId);

        await handler.Handle(command);

        reservaPendente.Status.Should().Be(StatusReserva.Confirmada);
        _mockReservaRepository.Verify(repo => repo.UpdateAsync(reservaPendente), Times.Once);

        _mockMessagePublisher.Verify(
            publisher => publisher.PublishAsync(It.IsAny<ReservaConfirmadaEvent>()),
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

        var handler = new ConfirmarReservaCommandHandler(
            _mockReservaRepository.Object, 
            _mockMessagePublisher.Object
        );
        var command = new ConfirmarReservaCommand(idInexistente);

        Func<Task> act = async () => await handler.Handle(command);
        
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Handle_QuandoReservaNaoEstaPendente_DeveLancarInvalidOperationException()
    {
        var reservaId = Guid.NewGuid();
        var reservaJaConfirmada = Reserva.Criar("Cliente Teste", DateTime.UtcNow.AddDays(1), 2);
        reservaJaConfirmada.Confirmar();

        _mockReservaRepository
            .Setup(repo => repo.GetByIdAsync(reservaId))
            .ReturnsAsync(reservaJaConfirmada);

        var handler = new ConfirmarReservaCommandHandler(
            _mockReservaRepository.Object, 
            _mockMessagePublisher.Object
        );
        var command = new ConfirmarReservaCommand(reservaId);

        Func<Task> act = async () => await handler.Handle(command);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Apenas reservas pendentes podem ser confirmadas.");
    }
}
