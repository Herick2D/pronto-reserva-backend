using FluentAssertions;
using Moq;
using ProntoReserva.Application.Features.Reservas.Commands.CancelarReserva;
using ProntoReserva.Domain.Entities;
using ProntoReserva.Domain.Enums;
using ProntoReserva.Domain.Repositories;

namespace ProntoReserva.Tests.Unit.Application.Features.Reservas.Commands.CancelarReserva;

public class CancelarReservaCommandHandlerTests
{
    private readonly Mock<IReservaRepository> _mockReservaRepository;

    public CancelarReservaCommandHandlerTests()
    {
        _mockReservaRepository = new Mock<IReservaRepository>();
    }

    [Fact]
    public async Task Handle_QuandoReservaExisteECancelavel_DeveCancelarEChamarUpdateAsync()
    {
        var reservaId = Guid.NewGuid();
        var reservaCancelavel = Reserva.Criar("Cliente Teste", DateTime.UtcNow.AddDays(1), 2);
        
        _mockReservaRepository
            .Setup(repo => repo.GetByIdAsync(reservaId))
            .ReturnsAsync(reservaCancelavel);

        var handler = new CancelarReservaCommandHandler(_mockReservaRepository.Object);
        var command = new CancelarReservaCommand(reservaId);

        await handler.Handle(command);

        reservaCancelavel.Status.Should().Be(StatusReserva.Cancelada);
        _mockReservaRepository.Verify(repo => repo.UpdateAsync(reservaCancelavel), Times.Once);
    }

    [Fact]
    public async Task Handle_QuandoReservaNaoExiste_DeveLancarKeyNotFoundException()
    {
        var idInexistente = Guid.NewGuid();
        
        _mockReservaRepository
            .Setup(repo => repo.GetByIdAsync(idInexistente))
            .ReturnsAsync((Reserva?)null);

        var handler = new CancelarReservaCommandHandler(_mockReservaRepository.Object);
        var command = new CancelarReservaCommand(idInexistente);

        Func<Task> act = async () => await handler.Handle(command);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    // TODO: Revisitar este teste após a conclusão dos requisitos principais.
    // Objetivo: Testar a regra de negócio que impede o cancelamento de uma reserva no estado "Concluida".
    // Problema Atual: O teste falha porque não há um método público na entidade `Reserva`
    // para transitar o estado para "Concluida". Isto é um bom design de domínio, mas torna
    // a criação deste cenário de teste específico difícil. A tentativa de forçar o erro
    // com `Moq` falha porque o método `Cancelar()` não é `virtual`.
    // Ação Futura: Avaliar se a entidade `Reserva` deve ter um método para marcar como concluída.
    // Se sim, reativar e ajustar este teste. Se não, este teste pode ser removido.
    /*
    [Fact]
    public async Task Handle_QuandoReservaNaoPodeSerCancelada_DeveLancarInvalidOperationException()
    {
        var reservaId = Guid.NewGuid();
        var reservaConcluida = Reserva.Criar("Cliente", DateTime.UtcNow.AddDays(1), 1);
        
        _mockReservaRepository
            .Setup(repo => repo.GetByIdAsync(reservaId))
            .ReturnsAsync(reservaConcluida);

        var handler = new CancelarReservaCommandHandler(_mockReservaRepository.Object);
        var command = new CancelarReservaCommand(reservaId);

        Func<Task> act = async () => await handler.Handle(command);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
    */
}
