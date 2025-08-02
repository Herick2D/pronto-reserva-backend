using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using ProntoReserva.Application.Features.Reservas.Commands.CreateReserva;
using ProntoReserva.Application.Features.Reservas.Commands.UpdateReserva;
using ProntoReserva.Application.Features.Reservas.Common;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace ProntoReserva.Tests.Integration.Reservas;

public class ReservasControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ReservasControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<Guid> CriarReservaParaTeste(string nomeCliente)
    {
        var command = new CreateReservaCommand(nomeCliente, DateTime.UtcNow.AddDays(1), 1, null);
        var response = await _client.PostAsJsonAsync("/api/reservas", command);
        response.EnsureSuccessStatusCode();
        var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
        return jsonResponse.GetProperty("id").GetGuid();
    }

    [Fact]
    public async Task PostReserva_ComDadosValidos_DeveRetornarCreated()
    {
        var command = new CreateReservaCommand("Cliente de Integração", DateTime.UtcNow.AddDays(10), 4, "Teste de integração");

        var response = await _client.PostAsJsonAsync("/api/reservas", command);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
    }

    [Fact]
    public async Task PostReserva_ComDadosInvalidos_DeveRetornarBadRequest()
    {
        var command = new CreateReservaCommand("Cliente Inválido", DateTime.UtcNow.AddDays(-1), 2, null);
        var response = await _client.PostAsJsonAsync("/api/reservas", command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetReservaById_QuandoReservaExiste_DeveRetornarOkComDadosCorretos()
    {
        var newId = await CriarReservaParaTeste("Cliente para Buscar");

        var getResponse = await _client.GetAsync($"/api/reservas/{newId}");

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var reservaResponse = await getResponse.Content.ReadFromJsonAsync<ReservaResponse>();
        reservaResponse?.Id.Should().Be(newId);
    }
    
    [Fact]
    public async Task GetAllReservas_QuandoExistemReservas_DeveRetornarOkComListaPaginada()
    {
        await CriarReservaParaTeste("Cliente na Lista");

        var response = await _client.GetAsync("/api/reservas?pageNumber=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var paginatedResponse = await response.Content.ReadFromJsonAsync<PaginatedResponse<ReservaResponse>>();
        paginatedResponse?.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task UpdateReserva_QuandoReservaExiste_DeveRetornarNoContent()
    {
        var idParaAtualizar = await CriarReservaParaTeste("Cliente Antigo");
        var command = new UpdateReservaCommand(idParaAtualizar, "Cliente Novo", DateTime.UtcNow.AddDays(5), 5, "Atualizado");

        var response = await _client.PutAsJsonAsync($"/api/reservas/{idParaAtualizar}", command);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/reservas/{idParaAtualizar}");
        var updatedReserva = await getResponse.Content.ReadFromJsonAsync<ReservaResponse>();
        updatedReserva?.NomeCliente.Should().Be("Cliente Novo");
    }

    [Fact]
    public async Task DeleteReserva_QuandoReservaExiste_DeveRetornarNoContent()
    {
        var idParaApagar = await CriarReservaParaTeste("Cliente a ser Apagado");

        var response = await _client.DeleteAsync($"/api/reservas/{idParaApagar}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/reservas/{idParaApagar}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
