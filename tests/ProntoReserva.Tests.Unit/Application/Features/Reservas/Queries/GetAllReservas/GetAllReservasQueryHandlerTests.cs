using FluentAssertions;
using Moq;
using ProntoReserva.Application.Features.Reservas.Queries.GetAllReservas;
using ProntoReserva.Domain.Entities;
using ProntoReserva.Domain.Repositories;

namespace ProntoReserva.Tests.Unit.Application.Features.Reservas.Queries.GetAllReservas;

public class GetAllReservasQueryHandlerTests
{
    private readonly Mock<IReservaRepository> _mockReservaRepository;

    public GetAllReservasQueryHandlerTests()
    {
        _mockReservaRepository = new Mock<IReservaRepository>();
    }
    
    private List<Reserva> GerarListaDeReservas(int quantidade)
    {
        var lista = new List<Reserva>();
        for (int i = 0; i < quantidade; i++)
        {
            lista.Add(Reserva.Criar($"Cliente {i + 1}", DateTime.UtcNow.AddDays(i + 1), 2));
        }
        return lista;
    }

    [Fact]
    public async Task Handle_QuandoExistemReservas_DeveRetornarRespostaPaginadaCorretamente()
    {
        var query = new GetAllReservasQuery(PageNumber: 1, PageSize: 5);
        var listaReservasFalsas = GerarListaDeReservas(5);
        var contagemTotalFalsa = 20;

        _mockReservaRepository
            .Setup(repo => repo.GetAllAsync(query.PageNumber, query.PageSize))
            .ReturnsAsync((listaReservasFalsas, contagemTotalFalsa));

        var handler = new GetAllReservasQueryHandler(_mockReservaRepository.Object);
        var result = await handler.Handle(query);

        result.Should().NotBeNull();
        result.Items.Should().HaveCount(5);
        result.TotalCount.Should().Be(contagemTotalFalsa);
        result.PageNumber.Should().Be(query.PageNumber);
        result.PageSize.Should().Be(query.PageSize);
        result.TotalPages.Should().Be(4);
        result.HasNextPage.Should().BeTrue();
        result.HasPreviousPage.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_QuandoNaoExistemReservas_DeveRetornarRespostaPaginadaVazia()
    {
        var query = new GetAllReservasQuery();

        _mockReservaRepository
            .Setup(repo => repo.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((new List<Reserva>(), 0));

        var handler = new GetAllReservasQueryHandler(_mockReservaRepository.Object);

        var result = await handler.Handle(query);

        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.TotalPages.Should().Be(0);
        result.HasNextPage.Should().BeFalse();
    }
}
