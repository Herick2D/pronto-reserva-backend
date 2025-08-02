using FluentAssertions;
using Moq;
using ProntoReserva.Application.Features.Reservas.Common;
using ProntoReserva.Application.Features.Reservas.Queries.GetReservaById;
using ProntoReserva.Domain.Entities;
using ProntoReserva.Domain.Repositories;

namespace ProntoReserva.Tests.Unit.Application.Features.Reservas.Queries.GetReservaById;

public class GetReservaByIdQueryHandlerTests
{
    private readonly Mock<IReservaRepository> _mockReservaRepository;

    public GetReservaByIdQueryHandlerTests()
    {
        _mockReservaRepository = new Mock<IReservaRepository>();
    }

    [Fact]
    public async Task Handle_QuandoReservaExiste_DeveRetornarReservaResponse()
    {
        var reservaId = Guid.NewGuid();
        var reservaFalsa = Reserva.Criar("Cliente Falso", DateTime.UtcNow.AddDays(1), 2);

        _mockReservaRepository
            .Setup(repo => repo.GetByIdAsync(reservaId))
            .ReturnsAsync(reservaFalsa);

        var handler = new GetReservaByIdQueryHandler(_mockReservaRepository.Object);

        var result = await handler.Handle(reservaId);

        result.Should().NotBeNull();
        result.Should().BeOfType<ReservaResponse>();
        result?.Id.Should().Be(reservaFalsa.Id);
        result?.NomeCliente.Should().Be("Cliente Falso");
    }

    [Fact]
    public async Task Handle_QuandoReservaNaoExiste_DeveRetornarNulo()
    {

        _mockReservaRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Reserva?)null);

        var handler = new GetReservaByIdQueryHandler(_mockReservaRepository.Object);
        var idInexistente = Guid.NewGuid();

        var result = await handler.Handle(idInexistente);

        result.Should().BeNull();
    }
}
