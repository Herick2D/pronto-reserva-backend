using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using ProntoReserva.Application.Features.Reservas.Commands.CreateReserva;
using ProntoReserva.Application.Features.Reservas.Commands.UpdateReserva;
using ProntoReserva.Application.Features.Reservas.Common;
using ProntoReserva.Application.Features.Users.Commands.Login;
using ProntoReserva.Application.Features.Users.Commands.Register;
using System.Net;
using System.Net.Http.Headers;
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
    
    private async Task AuthenticateClientAsync()
    {
        var email = $"teste-{Guid.NewGuid()}@testes.com";
        var password = "senhavalida123!";
        var registerCommand = new RegisterUserCommand(email, password);
        await _client.PostAsJsonAsync("/api/auth/register", registerCommand);

        var loginCommand = new LoginUserCommand(email, password);
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginCommand);
        loginResponse.EnsureSuccessStatusCode();
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginUserResponse>();

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.Token);
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
        await AuthenticateClientAsync();
        var command = new CreateReservaCommand("Cliente de Integração", DateTime.UtcNow.AddDays(10), 4, "Teste de integração");

        var response = await _client.PostAsJsonAsync("/api/reservas", command);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
    }

    [Fact]
    public async Task PostReserva_ComDadosInvalidos_DeveRetornarBadRequest()
    {
        await AuthenticateClientAsync();
        var command = new CreateReservaCommand("Cliente Inválido", DateTime.UtcNow.AddDays(-1), 2, null);

        var response = await _client.PostAsJsonAsync("/api/reservas", command);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetReservaById_QuandoReservaExiste_DeveRetornarOkComDadosCorretos()
    {
        await AuthenticateClientAsync();
        var newId = await CriarReservaParaTeste("Cliente para Buscar");

        var getResponse = await _client.GetAsync($"/api/reservas/{newId}");

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var reservaResponse = await getResponse.Content.ReadFromJsonAsync<ReservaResponse>();
        reservaResponse?.Id.Should().Be(newId);
    }
    
    [Fact]
    public async Task GetAllReservas_QuandoExistemReservas_DeveRetornarOkComListaPaginada()
    {
        await AuthenticateClientAsync();
        await CriarReservaParaTeste("Cliente na Lista");

        var response = await _client.GetAsync("/api/reservas?pageNumber=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var paginatedResponse = await response.Content.ReadFromJsonAsync<PaginatedResponse<ReservaResponse>>();
        paginatedResponse?.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task UpdateReserva_QuandoReservaExiste_DeveRetornarNoContent()
    {
        await AuthenticateClientAsync();
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
        await AuthenticateClientAsync();
        var idParaApagar = await CriarReservaParaTeste("Cliente a ser Apagado");

        var response = await _client.DeleteAsync($"/api/reservas/{idParaApagar}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/reservas/{idParaApagar}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
