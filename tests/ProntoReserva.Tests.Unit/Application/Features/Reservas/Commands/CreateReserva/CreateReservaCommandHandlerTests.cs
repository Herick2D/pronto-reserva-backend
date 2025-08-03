using FluentAssertions;
using Moq;
using ProntoReserva.Application.Abstractions.Authentication;
using ProntoReserva.Application.Features.Reservas.Commands.CreateReserva;
using ProntoReserva.Domain.Entities;
using ProntoReserva.Domain.Repositories;

namespace ProntoReserva.Tests.Unit.Application.Features.Reservas.Commands.CreateReserva;

public class CreateReservaCommandHandlerTests
{
    private readonly Mock<IReservaRepository> _mockReservaRepository;
    private readonly Mock<IUserContext> _mockUserContext;
    private readonly Guid _userId = Guid.NewGuid();

    public CreateReservaCommandHandlerTests()
    {
        _mockReservaRepository = new Mock<IReservaRepository>();
        _mockUserContext = new Mock<IUserContext>();

        _mockUserContext.Setup(x => x.GetUserId()).Returns(_userId);
    }

    [Fact]
    public async Task Handle_ComComandoValido_DeveChamarAddAsyncNoRepositorioUmaVez()
    {
        var handler = new CreateReservaCommandHandler(
            _mockReservaRepository.Object,
            _mockUserContext.Object
        );

        var command = new CreateReservaCommand(
            "Cliente Válido",
            DateTime.UtcNow.AddDays(5),
            2,
            "Observação de teste"
        );

        var reservaId = await handler.Handle(command);

        _mockReservaRepository.Verify(
            repo => repo.AddAsync(It.Is<Reserva>(r => r.UserId == _userId)),
            Times.Once
        );

        reservaId.Should().NotBeEmpty();
    }
}