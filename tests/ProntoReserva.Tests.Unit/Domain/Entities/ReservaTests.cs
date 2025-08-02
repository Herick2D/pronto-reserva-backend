using FluentAssertions;
using ProntoReserva.Domain.Entities;
using ProntoReserva.Domain.Enums;
using System;

namespace ProntoReserva.Tests.Unit.Domain.Entities;

public class ReservaTests
{
    private static Reserva CriarReservaValida() =>
        Reserva.Criar("Cliente Teste", DateTime.UtcNow.AddDays(1), 2);

    #region Testes do Método Criar

    [Fact]
    public void Criar_ComDadosValidos_DeveCriarReservaComSucesso()
    {
        var nomeCliente = "Herick Moreira";
        var dataReserva = DateTime.UtcNow.AddDays(1);
        var numeroPessoas = 4;
        
        var reserva = Reserva.Criar(nomeCliente, dataReserva, numeroPessoas);
        
        reserva.Should().NotBeNull();
        reserva.NomeCliente.Should().Be(nomeCliente);
        reserva.Status.Should().Be(StatusReserva.Pendente);
        reserva.DeletedAt.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Criar_ComNomeClienteVazioOuNulo_DeveLancarArgumentException(string nomeInvalido)
    {
        Action act = () => Reserva.Criar(nomeInvalido, DateTime.UtcNow.AddDays(1), 2);

        act.Should().Throw<ArgumentException>().And.ParamName.Should().Be("nomeCliente");
    }

    [Fact]
    public void Criar_ComDataNoPassado_DeveLancarArgumentException()
    {
        Action act = () => Reserva.Criar("Cliente", DateTime.UtcNow.AddMinutes(-1), 2);

        act.Should().Throw<ArgumentException>().And.ParamName.Should().Be("dataReserva");
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Criar_ComNumeroDePessoasInvalido_DeveLancarArgumentException(int numeroPessoasInvalido)
    {
        Action act = () => Reserva.Criar("Cliente", DateTime.UtcNow.AddDays(5), numeroPessoasInvalido);

        act.Should().Throw<ArgumentException>().And.ParamName.Should().Be("numeroPessoas");
    }

    #endregion

    #region Testes do Método Atualizar

    [Fact]
    public void Atualizar_ComDadosValidos_DeveAtualizarPropriedades()
    {
        var reserva = CriarReservaValida();
        var novoNome = "Cliente Atualizado";
        var novaData = DateTime.UtcNow.AddDays(10);
        var novoNumeroPessoas = 5;

        reserva.Atualizar(novoNome, novaData, novoNumeroPessoas);

        reserva.NomeCliente.Should().Be(novoNome);
        reserva.DataReserva.Should().Be(novaData);
        reserva.NumeroPessoas.Should().Be(novoNumeroPessoas);
    }
    #endregion

    #region Testes dos Métodos de Status

    [Fact]
    public void Confirmar_QuandoStatusEhPendente_DeveMudarStatusParaConfirmada()
    {
        var reserva = CriarReservaValida();
        reserva.Status.Should().Be(StatusReserva.Pendente);

        reserva.Confirmar();

        reserva.Status.Should().Be(StatusReserva.Confirmada);
    }

    [Fact]
    public void Confirmar_QuandoStatusNaoEhPendente_DeveLancarInvalidOperationException()
    {

        var reserva = CriarReservaValida();
        reserva.Confirmar();
        reserva.Status.Should().Be(StatusReserva.Confirmada);

        Action act = () => reserva.Confirmar();

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Cancelar_QuandoStatusNaoEhConcluida_DeveMudarStatusParaCancelada()
    {
        var reserva = CriarReservaValida();
        reserva.Status.Should().Be(StatusReserva.Pendente);

        reserva.Cancelar();

        reserva.Status.Should().Be(StatusReserva.Cancelada);
    }
    #endregion

    #region Testes do Método Apagar (Soft Delete)

    [Fact]
    public void Apagar_QuandoChamado_DeveDefinirDataDelecao()
    {
        var reserva = CriarReservaValida();
        reserva.DeletedAt.Should().BeNull();

        reserva.Apagar();

        reserva.DeletedAt.Should().NotBeNull();
        reserva.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
    #endregion
}
