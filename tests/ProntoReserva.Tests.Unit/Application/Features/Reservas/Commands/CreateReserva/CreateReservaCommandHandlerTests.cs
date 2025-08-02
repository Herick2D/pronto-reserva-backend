using FluentAssertions;
using Moq;
using ProntoReserva.Application.Features.Reservas.Commands.CreateReserva;
using ProntoReserva.Domain.Entities;
using ProntoReserva.Domain.Repositories;


namespace ProntoReserva.Tests.Unit.Application.Features.Reservas.Commands.CreateReserva;

public class CreateReservaCommandHandlerTests
{
    private readonly Mock<IReservaRepository> _mockReservaRepository;

    public CreateReservaCommandHandlerTests()
    {
        _mockReservaRepository = new Mock<IReservaRepository>();
    }

    [Fact]
    public async Task Handle_ComComandoValido_DeveChamarAddAsyncNoRepositorioUmaVez()
    {
        var handler = new CreateReservaCommandHandler(_mockReservaRepository.Object);

        var command = new CreateReservaCommand(
            "Cliente Válido",
            DateTime.UtcNow.AddDays(5),
            2,
            "Observação de teste"
        );

        var reservaId = await handler.Handle(command);

        _mockReservaRepository.Verify(
            repo => repo.AddAsync(It.IsAny<Reserva>()),
            Times.Once
        );

        reservaId.Should().NotBeEmpty();
    }
}
