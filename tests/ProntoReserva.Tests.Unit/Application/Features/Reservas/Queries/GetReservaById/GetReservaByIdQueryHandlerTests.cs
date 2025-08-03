using FluentAssertions;
using Moq;
using ProntoReserva.Application.Abstractions.Authentication;
using ProntoReserva.Application.Features.Reservas.Common;
using ProntoReserva.Application.Features.Reservas.Queries.GetReservaById;
using ProntoReserva.Domain.Entities;
using ProntoReserva.Domain.Repositories;

namespace ProntoReserva.Tests.Unit.Application.Features.Reservas.Queries.GetReservaById;

public class GetReservaByIdQueryHandlerTests
{
    private readonly Mock<IReservaRepository> _mockReservaRepository;
    private readonly Mock<IUserContext> _mockUserContext;
    private readonly Guid _userId = Guid.NewGuid();

    public GetReservaByIdQueryHandlerTests()
    {
        _mockReservaRepository = new Mock<IReservaRepository>();
        _mockUserContext = new Mock<IUserContext>();

        _mockUserContext.Setup(x => x.GetUserId()).Returns(_userId);
    }

    [Fact]
    public async Task Handle_QuandoReservaExiste_DeveRetornarReservaResponse()
    {
        var reservaId = Guid.NewGuid();
        var reservaFalsa = Reserva.Criar("Cliente Falso", DateTime.UtcNow.AddDays(1), 2, _userId);

        _mockReservaRepository
            .Setup(repo => repo.GetByIdAsync(reservaId, _userId))
            .ReturnsAsync(reservaFalsa);

        var handler = new GetReservaByIdQueryHandler(
            _mockReservaRepository.Object, 
            _mockUserContext.Object
        );

        var result = await handler.Handle(reservaId);

        result.Should().NotBeNull();
        result.Should().BeOfType<ReservaResponse>();
        result?.Id.Should().Be(reservaFalsa.Id);
        result?.NomeCliente.Should().Be("Cliente Falso");
    }

    [Fact]
    public async Task Handle_QuandoReservaNaoExiste_DeveRetornarNulo()
    {
        var idInexistente = Guid.NewGuid();

        _mockReservaRepository
            .Setup(repo => repo.GetByIdAsync(idInexistente, _userId))
            .ReturnsAsync((Reserva?)null);

        var handler = new GetReservaByIdQueryHandler(
            _mockReservaRepository.Object,
            _mockUserContext.Object
        );

        var result = await handler.Handle(idInexistente);

        result.Should().BeNull();
    }
}
